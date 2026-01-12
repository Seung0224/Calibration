using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
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

            using (Mat gray = _sourceImage.CvtColor(ColorConversionCodes.BGR2GRAY))
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

            for (int i = 0; i < corners.Length - 1; i++)
            {
                if ((i + 1) % PATTERN_W == 0) continue; // 줄바꿈 건너뜀

                double dPx = corners[i].DistanceTo(corners[i + 1]);
                double dMm = dPx * m_CalX;
                double error = Math.Abs(ACTUAL_SQUARE_SIZE - dMm);

                if (error > maxError) maxError = error;
                sumError += error;
                count++;
            }
            lstLog.Items.Add($"평균 오차: {sumError / count:F4} mm");
            lstLog.Items.Add($"최대 오차: {maxError:F4} mm");
            MessageBox.Show($"[Step 1 결과]\n최대 오차: {maxError:F4} mm");
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

                // Robot (Ideal mm)
                int row = i / PATTERN_W;
                int col = i % PATTERN_W;
                nRobotPos[i, 0] = col * ACTUAL_SQUARE_SIZE; // X (mm)
                nRobotPos[i, 1] = row * ACTUAL_SQUARE_SIZE; // Y (mm)
            }

            // 3. COMPANY 방식 Matrix 계산
            // 회사 코드는 3x3 Homography 행렬을 1차원 배열[9]로 풀어서 사용합니다.
            double[] nCalMatrix = new double[9];

            if (!My_COMPANY_Calibration_Matrix(nPixelPos, nRobotPos, nPoints, ref nCalMatrix))
            {
                MessageBox.Show("Matrix 계산 실패!");
                return;
            }

            // 4. 오차 검증 (COMPANY V2R 함수 사용)
            CalculateError_Step2_COMPANY(nPixelPos, nCalMatrix);
        }
        public bool My_COMPANY_Calibration_Matrix(double[,] nPixelPos, double[,] nRobotPos, int nCalCount, ref double[] nCalMatrix)
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
        public void My_COMPANY_V2R(double[] nCalMatrix, double nPixel_X, double nPixel_Y, ref double nRobot_X, ref double nRobot_Y)
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

            // nRobot_X *= 1000.0; 
            // nRobot_Y *= 1000.0;
        }
        private void CalculateError_Step2_COMPANY(double[,] nPixelPos, double[] nCalMatrix)
        {
            double maxError = 0, sumError = 0;
            int count = 0;

            for (int i = 0; i < nPixelPos.GetLength(0) - 1; i++)
            {
                if ((i + 1) % PATTERN_W == 0) continue;

                double rx1 = 0, ry1 = 0;
                double rx2 = 0, ry2 = 0;

                // COMPANY 방식 V2R 함수로 좌표 변환
                My_COMPANY_V2R(nCalMatrix, nPixelPos[i, 0], nPixelPos[i, 1], ref rx1, ref ry1);
                My_COMPANY_V2R(nCalMatrix, nPixelPos[i + 1, 0], nPixelPos[i + 1, 1], ref rx2, ref ry2);

                // mm 좌표계에서의 거리 측정
                double distMm = Math.Sqrt(Math.Pow(rx1 - rx2, 2) + Math.Pow(ry1 - ry2, 2));
                double error = Math.Abs(ACTUAL_SQUARE_SIZE - distMm);

                if (error > maxError) maxError = error;
                sumError += error;
                count++;
            }
            lstLog.Items.Add($"평균 오차: {sumError / count:F4} mm");
            lstLog.Items.Add($"최대 오차: {maxError:F4} mm");
            MessageBox.Show($"[Step 2 (회사 방식) 결과]\n최대 오차: {maxError:F4} mm\n\n확인 후 다음 단계로 넘어갑시다.");
        }
        #endregion

        // 설명
        // 1. 렌즈의 왜곡 계수(k1, k2...)와 내부 파라미터(focal length 등)를 추출
        // 2. 이미지를 Undistort(펴기) 처리
        // 3. 펴진 이미지로 다시 '회사 방식(Step 2)'을 돌려서 오차 0에 도전
        #region step3: Barrel Distrotion 보정 방식
        // 캘리브레이션 결과를 저장할 변수 (실제로는 파일로 저장해야 함)
        private Mat _camMatrix = new Mat();   // 카메라 내부 파라미터 (f_x, f_y, c_x, c_y)
        private Mat _distCoeffs = new Mat();  // 왜곡 계수 (k_1, k_2, p_1, p_2...)
        private void BTN_Barrel_Distrotion_Verify_Click(object sender, EventArgs e)
        {
            if (_sourceImage == null) return;
            lstLog.Items.Add(">>> Step 3 (렌즈 왜곡 보정) 시작...");

            // 1. 코너 검출 (원본 이미지) -> 함수 재사용!
            if (!FindAndSubPixelCorners(_sourceImage, out Point2f[] corners)) return;

            // 2. 3D Object Point 생성
            List<Point3f> objectPoints = new List<Point3f>();
            for (int i = 0; i < PATTERN_H; i++)
            {
                for (int j = 0; j < PATTERN_W; j++)
                {
                    objectPoints.Add(new Point3f((float)j * (float)ACTUAL_SQUARE_SIZE, (float)i * (float)ACTUAL_SQUARE_SIZE, 0));
                }
            }

            // List -> Mat 변환
            Mat objectPointsMat = InputArray.Create(objectPoints.ToArray()).GetMat().Clone();
            Mat cornersMat = InputArray.Create(corners).GetMat().Clone();

            // 3. 카메라 캘리브레이션 실행
            Mat[] rvecs, tvecs;
            double rms = Cv2.CalibrateCamera(new List<Mat> { objectPointsMat }, new List<Mat> { cornersMat },_sourceImage.Size(), _camMatrix, _distCoeffs, out rvecs, out tvecs);

            objectPointsMat.Dispose();
            cornersMat.Dispose();

            lstLog.Items.Add($"Calibration RMS 오차: {rms:F4}");
            lstLog.Items.Add("왜곡 계수 추출 완료.");

            // 4. 이미지 펴기 (Undistort)
            Mat undistortedImg = new Mat();
            // _camMatrix와 _distCoeffs 값만 필요
            Cv2.Undistort(_sourceImage, undistortedImg, _camMatrix, _distCoeffs);

            lstLog.Items.Add("이미지 Undistort 완료.");

            // 5. 재검증 (펴진 이미지로 Step 2 재실행) -> 함수 재사용!
            // 여기서 undistortedImg를 인자로 넘겨줍니다.
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
                    nRobotPos[i, 0] = col * ACTUAL_SQUARE_SIZE;
                    nRobotPos[i, 1] = row * ACTUAL_SQUARE_SIZE;
                }

                double[] nCalMatrix = new double[9];
                if (My_COMPANY_Calibration_Matrix(nPixelPos, nRobotPos, nPoints, ref nCalMatrix))
                {
                    CalculateError_Step2_COMPANY(nPixelPos, nCalMatrix);
                }
            }
        }

        #endregion
    }
}