using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using OpenCvSharp; // NuGet: OpenCvSharp4.Windows 필수
using OpenCvSharp.Extensions;

namespace Calibration
{
    public partial class MainForm : Form
    {
        private const double ACTUAL_SQUARE_SIZE = 30.0; // mm (체커보드 사각형 한 칸의 실제 크기)

        // 체커보드 내부 코너 개수 (가로 10칸이면 코너는 9개)
        // 회사기준이면 3*3 체커보드 기준
        private const int PATTERN_W = 9;
        private const int PATTERN_H = 6;

        private Mat _sourceImage; // 메모리 로드 이미지

        public MainForm()
        {
            InitializeComponent();
        }

        #region Common Functions
        private void BTN_Open_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "Image Files|*.jpg;*.png;*.bmp";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    _sourceImage?.Dispose();
                    _sourceImage = Cv2.ImRead(dlg.FileName);
                    imageBox1.Image = _sourceImage.ToBitmap();

                    lstLog.Items.Clear();
                    lstLog.Items.Add($"[이미지 로드] {dlg.SafeFileName}");
                    lstLog.Items.Add($"해상도: {_sourceImage.Width} x {_sourceImage.Height}");
                }
            }
        }
        private bool FindAndSubPixelCorners(out Point2f[] corners)
        {
            return FindAndSubPixelCorners(_sourceImage, out corners);
        }
        private bool FindAndSubPixelCorners(Mat targetImg, out Point2f[] corners)
        {
            bool found = Cv2.FindChessboardCorners(targetImg, new OpenCvSharp.Size(PATTERN_W, PATTERN_H), out corners);
            if (!found)
            {
                // 실패 시 로그만 남기고 메시지박스는 띄우지 않음 (재검증 과정 등을 위해)
                lstLog.Items.Add("코너 검출 실패");
                corners = null;
                return false;
            }

            using (Mat gray = targetImg.CvtColor(ColorConversionCodes.BGR2GRAY))
            {
                Cv2.CornerSubPix(gray, corners, new OpenCvSharp.Size(11, 11), new OpenCvSharp.Size(-1, -1),
                    new TermCriteria(CriteriaTypes.Eps | CriteriaTypes.MaxIter, 100, 0.001));
            }

            // 시각화 (현재 처리 중인 이미지를 화면에 갱신)
            Mat res = targetImg.Clone();
            Cv2.DrawChessboardCorners(res, new OpenCvSharp.Size(PATTERN_W, PATTERN_H), corners, found);
            imageBox1.Image = res.ToBitmap();

            return true;
        }
        #endregion

        // 설명
        // 카메라가의 위치가 물체의 정중앙, 회전 틀어짐 없이, 기울어짐 없을때 (물리적으로 불가능)
        #region step 1: 단순 비례 방식
        private void BTN_Verify_Click(object sender, EventArgs e)
        {
            if (_sourceImage == null) return;
            lstLog.Items.Add(">>> Step 1 (단순 비례) 검증 시작...");

            // 1. 코너 검출
            if (!FindAndSubPixelCorners(out Point2f[] corners)) return;

            // 2. 중앙부 기준 배율 계산
            int centerIdx1 = 22;
            int centerIdx2 = 23;
            double pixelDist = Math.Sqrt(Math.Pow(corners[centerIdx1].X - corners[centerIdx2].X, 2) + Math.Pow(corners[centerIdx1].Y - corners[centerIdx2].Y, 2));
            double m_CalX = ACTUAL_SQUARE_SIZE / pixelDist;

            lstLog.Items.Add($"[기준] 1px 당 길이: {m_CalX:F5} mm/px");

            // 3. 전체 오차 검증
            CalculateError_Step1(corners, m_CalX);
        }
        private void CalculateError_Step1(Point2f[] corners, double m_CalX)
        {
            double maxError = 0, sumError = 0;
            int count = 0;

            dgvDistances.Rows.Clear();

            for (int i = 0; i < corners.Length - 1; i++)
            {
                if ((i + 1) % PATTERN_W == 0) continue; // 줄바꿈 건너뜀

                double dPx = corners[i].DistanceTo(corners[i + 1]);
                double dMm = dPx * m_CalX;
                double error = Math.Abs(ACTUAL_SQUARE_SIZE - dMm);

                if (error > maxError) maxError = error;
                sumError += error;
                count++;

                dgvDistances.Rows.Add(
                    i,
                    $"({corners[i].X:F1}, {corners[i].Y:F1})",
                    $"({corners[i + 1].X:F1}, {corners[i + 1].Y:F1})",
                    $"{dMm:F3}mm",
                    $"{error:F3}mm");
            }
            lstLog.Items.Add($"평균 오차: {sumError / count:F4} mm");
            lstLog.Items.Add($"최대 오차: {maxError:F4} mm");
        }
        #endregion

        // 설명
        //1. Shift (위치 틀어짐):카메라가 물체 정중앙에 있지않고 옆으로 치우쳐 있을때 (좌표 이동 보정)
        //2. Rotation (회전 틀어짐): 카메라 자체가 회전이 들어가있을때
        //3. Ttilt (기울어짐/원근감): 카메라가 물체를 수직으로 내려다보지않고 비스듬하게 보고있을때
        #region step 2: 회사 COMPANY 방식
        private void BTN_Company_Verify_Click(object sender, EventArgs e)
        {
            if (_sourceImage == null) return;
            lstLog.Items.Add(">>> Step 2 (회사 COMPANY 방식) 검증 시작...");

            // 1. 코너 검출 (Vision 좌표)
            if (!FindAndSubPixelCorners(out Point2f[] corners)) return;

            // 2. 정답지 (Robot 좌표 / Real World) 생성
            // 회사 코드의 'nRobotPos'를 만드는 과정입니다. (0,0), (30,0), (60,0)...
            int nPoints = corners.Length;
            double[,] nPixelPos = new double[nPoints, 2];
            double[,] nRobotPos = new double[nPoints, 2];

            for (int i = 0; i < nPoints; i++)
            {
                // Pixel (Vision)
                nPixelPos[i, 0] = corners[i].X;
                nPixelPos[i, 1] = corners[i].Y;

                // Robot (Ideal, m 단위 - BSI_V2R가 내부에서 ×1000으로 mm 복원)
                int row = i / PATTERN_W;
                int col = i % PATTERN_W;
                nRobotPos[i, 0] = (col * ACTUAL_SQUARE_SIZE) / 1000.0; // X (m)
                nRobotPos[i, 1] = (row * ACTUAL_SQUARE_SIZE) / 1000.0; // Y (m)
            }

            // 3. COMPANY 방식 Matrix 계산
            // 회사 코드는 3x3 Homography 행렬을 1차원 배열[9]로 풀어서 사용합니다.
            double[] nCalMatrix = new double[9];

            if (!BSI_Calibration_Matrix(nPixelPos, nRobotPos, nPoints, ref nCalMatrix))
            {
                MessageBox.Show("Matrix 계산 실패!");
                return;
            }

            // 4. 오차 검증 (COMPANY V2R 함수 사용)
            CalculateError_Step2_COMPANY(nPixelPos, nCalMatrix, dgvHomography);
        }
        public bool BSI_Calibration_Matrix(double[,] nPixelPos, double[,] nRobotPos, int nCalCount, ref double[] nCalMatrix)
        {
            try
            {
                // 데이터를 OpenCV 포맷으로 변환
                List<Point2f> srcPts = new List<Point2f>();
                List<Point2f> dstPts = new List<Point2f>();

                for (int i = 0; i < nCalCount; i++)
                {
                    srcPts.Add(new Point2f((float)nPixelPos[i, 0], (float)nPixelPos[i, 1]));
                    dstPts.Add(new Point2f((float)nRobotPos[i, 0], (float)nRobotPos[i, 1]));
                }

                // [수정 포인트] List를 Array로 변환 후 InputArray.Create로 감싸야 함
                // OpenCvSharp4에서는 명시적으로 InputArray 타입을 요구하는 경우가 많습니다.
                Mat hMat = Cv2.FindHomography(InputArray.Create(srcPts.ToArray()), InputArray.Create(dstPts.ToArray()), HomographyMethods.LMedS);

                if (hMat.Empty()) return false;

                // [매핑] 3x3 행렬 -> 회사 포맷 1차원 배열(9개)
                nCalMatrix = new double[9];
                nCalMatrix[0] = hMat.At<double>(0, 0);
                nCalMatrix[1] = hMat.At<double>(0, 1);
                nCalMatrix[2] = hMat.At<double>(0, 2);
                nCalMatrix[3] = hMat.At<double>(1, 0);
                nCalMatrix[4] = hMat.At<double>(1, 1);
                nCalMatrix[5] = hMat.At<double>(1, 2);
                nCalMatrix[6] = hMat.At<double>(2, 0);
                nCalMatrix[7] = hMat.At<double>(2, 1);
                nCalMatrix[8] = 1.0;

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Err: " + ex.Message);
                return false;
            }
        }
        public void BSI_V2R(double[] nCalMatrix, double nPixel_X, double nPixel_Y, ref double nRobot_X, ref double nRobot_Y)
        {
            // 디컴파일된 수식 그대로 적용
            nRobot_X = nPixel_X * nCalMatrix[0] + nPixel_Y * nCalMatrix[1] + nCalMatrix[2];
            nRobot_Y = nPixel_X * nCalMatrix[3] + nPixel_Y * nCalMatrix[4] + nCalMatrix[5];

            // [핵심] 분모 계산 (Perspective/원근 보정)
            // Affine 변환과 달리, 깊이(Depth)나 기울어짐을 보정하기 위해 나눗셈을 합니다.
            double num = nPixel_X * nCalMatrix[6] + nPixel_Y * nCalMatrix[7] + 1.0;

            nRobot_X /= num;
            nRobot_Y /= num;

            // [중요 체크포인트] COMPANY 원본에는 '* 1000.0'이 있습니다.
            // 하지만 지금 검증에서는 mm(30)를 넣고 mm(30)가 나오는지 확인해야 하므로
            // 이 1000배 튀기는 코드는 주석 처리해야 정확한 오차 계산이 됩니다.
            // (현장 장비에서는 m 단위를 mm로 바꾸거나 단위를 맞추기 위해 쓴 것으로 추정됨)

            nRobot_X *= 1000.0; 
            nRobot_Y *= 1000.0;
        }
        private void CalculateError_Step2_COMPANY(double[,] nPixelPos, double[] nCalMatrix, DataGridView targetGrid)
        {
            double maxError = 0, sumError = 0;
            int count = 0;

            targetGrid.Rows.Clear();

            for (int i = 0; i < nPixelPos.GetLength(0) - 1; i++)
            {
                if ((i + 1) % PATTERN_W == 0) continue;

                double rx1 = 0, ry1 = 0;
                double rx2 = 0, ry2 = 0;

                // COMPANY 방식 V2R 함수로 좌표 변환
                BSI_V2R(nCalMatrix, nPixelPos[i, 0], nPixelPos[i, 1], ref rx1, ref ry1);
                BSI_V2R(nCalMatrix, nPixelPos[i + 1, 0], nPixelPos[i + 1, 1], ref rx2, ref ry2);

                // mm 좌표계에서의 거리 측정
                double distMm = Math.Sqrt(Math.Pow(rx1 - rx2, 2) + Math.Pow(ry1 - ry2, 2));
                double error = Math.Abs(ACTUAL_SQUARE_SIZE - distMm);

                if (error > maxError) maxError = error;
                sumError += error;
                count++;

                targetGrid.Rows.Add(
                    i,
                    $"({nPixelPos[i, 0]:F1}, {nPixelPos[i, 1]:F1})",
                    $"({nPixelPos[i + 1, 0]:F1}, {nPixelPos[i + 1, 1]:F1})",
                    $"{distMm:F3}mm",
                    $"{error:F3}mm");
            }
            lstLog.Items.Add($"평균 오차: {sumError / count:F4} mm");
            lstLog.Items.Add($"최대 오차: {maxError:F4} mm");
        }
        #endregion

        // 설명
        // 1. [다중 이미지 캘리브레이션] 여러 장의 체커보드 이미지로 카메라 내부 파라미터(camMatrix)와 왜곡 계수(distCoeffs)를 구해서 텍스트로 저장
        // 2. [캘리브레이션 데이터 로드] 저장해둔 텍스트를 다시 불러와 camMatrix/distCoeffs 복원 (매번 재계산할 필요 없음)
        // 3. [왜곡 보정 오차 검증] 캘리브레이션 데이터가 로드된 상태에서만 동작 - 테스트 이미지를 Undistort 후 Step 2를 재실행해서 오차 0에 도전
        #region step3: Barrel Distrotion 보정 방식
        private Mat _camMatrix = new Mat();   // 카메라 내부 파라미터 (f_x, f_y, c_x, c_y)
        private Mat _distCoeffs = new Mat();  // 왜곡 계수 (k_1, k_2, p_1, p_2...)
        private bool _isCalibrationDataLoaded = false; // 다중 이미지 캘리브레이션 또는 파일 로드가 완료됐는지

        private void BTN_MultiCalibrate_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "Image Files|*.jpg;*.png;*.bmp";
                dlg.Multiselect = true;
                if (dlg.ShowDialog() != DialogResult.OK) return;

                lstLog.Items.Clear();
                lstLog.Items.Add($">>> 다중 이미지 캘리브레이션 시작... ({dlg.FileNames.Length}장 선택됨)");

                // 모든 이미지에서 공통으로 쓰는 이상적 3D 격자 (평면, Z=0)
                List<Point3f> objectPoints = new List<Point3f>();
                for (int i = 0; i < PATTERN_H; i++)
                    for (int j = 0; j < PATTERN_W; j++)
                        objectPoints.Add(new Point3f((float)j * (float)ACTUAL_SQUARE_SIZE, (float)i * (float)ACTUAL_SQUARE_SIZE, 0));

                List<Mat> objectPointsList = new List<Mat>();
                List<Mat> imagePointsList = new List<Mat>();
                OpenCvSharp.Size imageSize = new OpenCvSharp.Size();

                foreach (string file in dlg.FileNames)
                {
                    using (Mat img = Cv2.ImRead(file))
                    {
                        imageSize = img.Size();

                        bool found = Cv2.FindChessboardCorners(img, new OpenCvSharp.Size(PATTERN_W, PATTERN_H), out Point2f[] corners);
                        if (!found)
                        {
                            lstLog.Items.Add($"  [실패] {System.IO.Path.GetFileName(file)} - 코너 검출 안 됨");
                            continue;
                        }

                        using (Mat gray = img.CvtColor(ColorConversionCodes.BGR2GRAY))
                        {
                            Cv2.CornerSubPix(gray, corners, new OpenCvSharp.Size(11, 11), new OpenCvSharp.Size(-1, -1),
                                new TermCriteria(CriteriaTypes.Eps | CriteriaTypes.MaxIter, 100, 0.001));
                        }

                        objectPointsList.Add(InputArray.Create(objectPoints.ToArray()).GetMat().Clone());
                        imagePointsList.Add(InputArray.Create(corners).GetMat().Clone());
                        lstLog.Items.Add($"  [성공] {System.IO.Path.GetFileName(file)}");
                    }
                }

                if (objectPointsList.Count < 3)
                {
                    MessageBox.Show("코너 검출에 성공한 이미지가 너무 적습니다 (최소 3장 이상 필요).");
                    foreach (var m in objectPointsList) m.Dispose();
                    foreach (var m in imagePointsList) m.Dispose();
                    return;
                }

                Mat[] rvecs, tvecs;
                double rms = Cv2.CalibrateCamera(objectPointsList, imagePointsList, imageSize, _camMatrix, _distCoeffs, out rvecs, out tvecs);

                foreach (var m in objectPointsList) m.Dispose();
                foreach (var m in imagePointsList) m.Dispose();

                lstLog.Items.Add($"캘리브레이션 완료: {imagePointsList.Count}장 사용, RMS 오차 {rms:F4}");

                string folder = System.IO.Path.GetDirectoryName(dlg.FileNames[0]);
                string savePath = System.IO.Path.Combine(folder, "calibration_data.txt");
                SaveCalibrationData(savePath, imageSize, rms);

                _isCalibrationDataLoaded = true;
                lstLog.Items.Add($"저장됨: {savePath}");
                MessageBox.Show($"캘리브레이션 완료!\n사용된 이미지: {imagePointsList.Count}장\nRMS 오차: {rms:F4}\n저장 경로: {savePath}");
            }
        }

        private void SaveCalibrationData(string path, OpenCvSharp.Size imageSize, double rms)
        {
            double[] camVals = {
                _camMatrix.At<double>(0,0), _camMatrix.At<double>(0,1), _camMatrix.At<double>(0,2),
                _camMatrix.At<double>(1,0), _camMatrix.At<double>(1,1), _camMatrix.At<double>(1,2),
                _camMatrix.At<double>(2,0), _camMatrix.At<double>(2,1), _camMatrix.At<double>(2,2)
            };

            int coeffCount = (int)_distCoeffs.Total();
            double[] distVals = new double[coeffCount];
            for (int i = 0; i < coeffCount; i++) distVals[i] = _distCoeffs.At<double>(i);

            List<string> lines = new List<string>
            {
                "CAMMATRIX=" + string.Join(",", camVals.Select(v => v.ToString(CultureInfo.InvariantCulture))),
                "DISTCOEFFS=" + string.Join(",", distVals.Select(v => v.ToString(CultureInfo.InvariantCulture))),
                $"IMAGESIZE={imageSize.Width},{imageSize.Height}",
                "RMS=" + rms.ToString(CultureInfo.InvariantCulture)
            };

            System.IO.File.WriteAllLines(path, lines);
        }

        private void BTN_LoadCalibration_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "Calibration Data|*.txt";
                if (dlg.ShowDialog() != DialogResult.OK) return;

                string[] lines = System.IO.File.ReadAllLines(dlg.FileName);
                double[] camVals = null, distVals = null;
                string imageSizeText = "", rmsText = "";

                foreach (string line in lines)
                {
                    if (line.StartsWith("CAMMATRIX="))
                        camVals = line.Substring("CAMMATRIX=".Length).Split(',').Select(s => double.Parse(s, CultureInfo.InvariantCulture)).ToArray();
                    else if (line.StartsWith("DISTCOEFFS="))
                        distVals = line.Substring("DISTCOEFFS=".Length).Split(',').Select(s => double.Parse(s, CultureInfo.InvariantCulture)).ToArray();
                    else if (line.StartsWith("IMAGESIZE="))
                        imageSizeText = line.Substring("IMAGESIZE=".Length);
                    else if (line.StartsWith("RMS="))
                        rmsText = line.Substring("RMS=".Length);
                }

                if (camVals == null || camVals.Length != 9 || distVals == null)
                {
                    MessageBox.Show("캘리브레이션 데이터 파일 형식이 올바르지 않습니다.");
                    return;
                }

                _camMatrix = new Mat(3, 3, MatType.CV_64F);
                for (int r = 0; r < 3; r++)
                    for (int c = 0; c < 3; c++)
                        _camMatrix.Set(r, c, camVals[r * 3 + c]);

                _distCoeffs = new Mat(1, distVals.Length, MatType.CV_64F);
                for (int i = 0; i < distVals.Length; i++)
                    _distCoeffs.Set(0, i, distVals[i]);

                _isCalibrationDataLoaded = true;

                lstLog.Items.Add($">>> 캘리브레이션 데이터 로드 완료: {System.IO.Path.GetFileName(dlg.FileName)}");
                if (!string.IsNullOrEmpty(imageSizeText)) lstLog.Items.Add($"  캘리브레이션 당시 해상도: {imageSizeText}");
                if (!string.IsNullOrEmpty(rmsText)) lstLog.Items.Add($"  캘리브레이션 RMS 오차: {rmsText}");

                MessageBox.Show("캘리브레이션 데이터를 불러왔습니다.");
            }
        }

        private void BTN_Barrel_Distrotion_Verify_Click(object sender, EventArgs e)
        {
            if (_sourceImage == null) return;

            if (!_isCalibrationDataLoaded)
            {
                MessageBox.Show("먼저 [다중 이미지 캘리브레이션]을 실행하거나 [캘리브레이션 데이터 로드]로 불러와주세요.");
                return;
            }

            lstLog.Items.Add(">>> Step 3 (렌즈 왜곡 보정) 검증 시작...");

            // 1. 이미지 펴기 (Undistort) - 이미 확보된 _camMatrix/_distCoeffs 사용
            Mat undistortedImg = new Mat();
            Cv2.Undistort(_sourceImage, undistortedImg, _camMatrix, _distCoeffs);
            lstLog.Items.Add("이미지 Undistort 완료.");

            // 2. 재검증 (펴진 이미지로 Step 2 재실행) -> 함수 재사용!
            if (FindAndSubPixelCorners(undistortedImg, out Point2f[] newCorners))
            {
                int nPoints = newCorners.Length;
                double[,] nPixelPos = new double[nPoints, 2];
                double[,] nRobotPos = new double[nPoints, 2];

                for (int i = 0; i < nPoints; i++)
                {
                    nPixelPos[i, 0] = newCorners[i].X;
                    nPixelPos[i, 1] = newCorners[i].Y;

                    int row = i / PATTERN_W;
                    int col = i % PATTERN_W;
                    nRobotPos[i, 0] = (col * ACTUAL_SQUARE_SIZE) / 1000.0; // X (m)
                    nRobotPos[i, 1] = (row * ACTUAL_SQUARE_SIZE) / 1000.0; // Y (m)
                }

                double[] nCalMatrix = new double[9];
                if (BSI_Calibration_Matrix(nPixelPos, nRobotPos, nPoints, ref nCalMatrix))
                {
                    CalculateError_Step2_COMPANY(nPixelPos, nCalMatrix, dgvDistortion);
                }
            }
        }

        #endregion

        private void BTN_COMPARE_Click(object sender, EventArgs e)
        {
            if (_sourceImage == null)
            {
                MessageBox.Show("먼저 이미지를 로드해주세요.");
                return;
            }

            lstLog.Items.Add(">>> [수학적 일치도 검증] 내 V2R 수식 vs 회사 DLL 비교...");

            // 1. 코너 검출
            if (!FindAndSubPixelCorners(_sourceImage, out Point2f[] corners)) return;

            // 2. 회사 DLL용 데이터 준비 (m 단위)
            int nPoints = corners.Length;
            double[,] nPixelPos = new double[nPoints, 2];
            double[,] nRobotPos = new double[nPoints, 2];

            for (int i = 0; i < nPoints; i++)
            {
                nPixelPos[i, 0] = corners[i].X;
                nPixelPos[i, 1] = corners[i].Y;

                int row = i / PATTERN_W;
                int col = i % PATTERN_W;
                // 회사 DLL은 m 단위를 기준으로 하므로 1000으로 나눔
                nRobotPos[i, 0] = (col * ACTUAL_SQUARE_SIZE) / 1000.0;
                nRobotPos[i, 1] = (row * ACTUAL_SQUARE_SIZE) / 1000.0;
            }

            // 3. 회사 DLL로 Matrix 계산
            JAS.Calibration.Calibration jasCal = new JAS.Calibration.Calibration();
            double[] jasMatrix = new double[9];
            int jasResult = jasCal.Calibration_Matrix(nPixelPos, nRobotPos, nPoints, ref jasMatrix);

            if (jasResult != 0)
            {
                MessageBox.Show("회사 DLL Matrix 계산 실패");
                return;
            }

            // -------------------------------------------------------------
            // [핵심 검증] 동일한 Matrix를 주입했을 때 결과 비교
            // -------------------------------------------------------------
            lstLog.Items.Add("--- 모든 코너 점에 대한 수학적 일치도 확인 ---");

            double maxDiff = 0.0;
            double sumDiff = 0.0;

            for (int i = 0; i < nPoints; i++)
            {
                double testX = nPixelPos[i, 0];
                double testY = nPixelPos[i, 1];

                // A. 회사 Matrix를 '나의 V2R' 수식에 주입
                double myRx = 0, myRy = 0;
                BSI_V2R(jasMatrix, testX, testY, ref myRx, ref myRy);

                // B. 회사 Matrix를 '회사 DLL V2R' 함수로 실행
                double jasRx = 0, jasRy = 0;
                jasCal.V2R(jasMatrix, testX, testY, ref jasRx, ref jasRy);

                // 두 결과의 거리 차이 계산
                double diff = Math.Sqrt(Math.Pow(myRx - jasRx, 2) + Math.Pow(myRy - jasRy, 2));

                if (diff > maxDiff) maxDiff = diff;
                sumDiff += diff;
            }

            double avgDiff = sumDiff / nPoints;

            lstLog.Items.Add($"평균 수학적 차이: {avgDiff:F9} mm");
            lstLog.Items.Add($"최대 수학적 차이: {maxDiff:F9} mm");

            // -------------------------------------------------------------
            // [최종 판정]
            // -------------------------------------------------------------
            // 소수점 6자리 이하의 차이는 컴퓨터 부동소수점 오차이므로 0으로 간주합니다.
            if (maxDiff < 0.000001)
            {
                lstLog.Items.Add(">>> [증명 완료] 나의 V2R 수식은 회사 표준과 100% 동일합니다.");
                MessageBox.Show($"[수학적 검증 성공]\n\n최대 차이: {maxDiff:F9} mm\n\n내 수식이 회사 DLL 로직과 완벽히 일치함이 증명되었습니다.");
            }
            else
            {
                lstLog.Items.Add(">>> [검증 실패] 수식이나 단위(1000배)를 다시 확인하세요.");
                MessageBox.Show($"[검증 실패]\n차이가 너무 큽니다: {maxDiff:F4} mm");
            }
        }
    }
}