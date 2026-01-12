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
        // =================================================================================
        // [1. 설정 영역] 실제 현장의 체커보드 규격 입력
        // =================================================================================
        // 중요: 이 값이 틀리면 모든 계산이 틀립니다. 버니어 캘리퍼스로 잰 실제 간격을 입력하세요.
        private const double ACTUAL_SQUARE_SIZE = 30.0; // mm (체커보드 사각형 한 칸의 실제 크기)

        // 체커보드 내부의 '코너 점' 개수입니다. (사각형 개수가 아님)
        // 예: 가로 사각형이 10개면 코너는 9개입니다.
        private const int PATTERN_W = 9;
        private const int PATTERN_H = 6;

        private Mat _sourceImage; // 메모리에 로드된 원본 이미지

        public MainForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 이미지 불러오기 버튼
        /// </summary>
        private void BTN_Open_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "Image Files|*.jpg;*.png;*.bmp";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    _sourceImage?.Dispose();

                    // OpenCV Mat 형식으로 이미지 로드
                    _sourceImage = Cv2.ImRead(dlg.FileName);

                    // 화면(PictureBox)에 띄우기 위해 Bitmap으로 변환
                    imageBox1.Image = _sourceImage.ToBitmap();

                    lstLog.Items.Clear();
                    lstLog.Items.Add($"[이미지 로드] {dlg.SafeFileName}");
                    lstLog.Items.Add($"해상도: {_sourceImage.Width} x {_sourceImage.Height}");
                }
            }
        }


        /// <summary>
        /// [Step 1 핵심] 단순 비례식 검증 버튼
        /// </summary>
        private void BTN_Verify_Click(object sender, EventArgs e)
        {
            if (_sourceImage == null) return;

            lstLog.Items.Add(">>> Step 1 검증 시작...");

            // =================================================================================
            // [2. 코너 검출] OpenCV가 체커보드의 교차점을 찾습니다.
            // =================================================================================
            Point2f[] corners;
            bool found = Cv2.FindChessboardCorners(_sourceImage, new OpenCvSharp.Size(PATTERN_W, PATTERN_H), out corners);

            if (!found)
            {
                MessageBox.Show("체커보드 코너를 찾지 못했습니다.\n조명을 확인하거나 패턴 개수 설정을 확인하세요.");
                return;
            }

            // =================================================================================
            // [3. 정밀 보정 (SubPixel)] - 머신비전의 필수 과정
            // =================================================================================
            // 그냥 찾으면 정수형(int) 픽셀만 나옵니다. (예: 100, 200)
            // 이를 주변 픽셀의 밝기 변화를 분석해 소수점(float) 단위까지 정밀하게 다듬습니다. (예: 100.42, 200.15)
            using (Mat gray = _sourceImage.CvtColor(ColorConversionCodes.BGR2GRAY))
            {
                Cv2.CornerSubPix(gray, corners, new OpenCvSharp.Size(11, 11), new OpenCvSharp.Size(-1, -1), new TermCriteria(CriteriaTypes.Eps | CriteriaTypes.MaxIter, 30, 0.1));
            }

            // =================================================================================
            // [4. 단순 비례식 계산] (중앙부 기준)
            // =================================================================================
            // 렌즈는 중앙부가 가장 왜곡이 적습니다. 
            // 따라서 9x6 패턴의 중앙에 위치한 22번, 23번 점을 기준으로 '1픽셀당 몇 mm인지' 계산합니다.

            // 인덱스 계산: (Height/2) * Width + (Width/2) 얼추 중앙
            int centerIdx1 = 22;
            int centerIdx2 = 23;

            Point2f p1 = corners[centerIdx1];
            Point2f p2 = corners[centerIdx2];

            // 픽셀 거리 계산 (피타고라스)
            double pixelDist = Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));

            // [핵심] m_CalX: 1픽셀이 실제 몇 mm인가? (Resolution)
            double m_CalX = ACTUAL_SQUARE_SIZE / pixelDist;

            lstLog.Items.Add($"[기준] 중앙부 픽셀거리: {pixelDist:F2}px");
            lstLog.Items.Add($"[기준] 1px 당 길이(분해능): {m_CalX:F5} mm/px");
            lstLog.Items.Add("--------------------------------------");

            // =================================================================================
            // [5. 전체 영역 오차 검증]
            // =================================================================================
            // 위에서 구한 m_CalX를 가지고, 가장자리(Edge) 점들의 거리를 재봅니다.
            // 렌즈 왜곡이 있다면, 가장자리로 갈수록 30mm가 안 나오거나 넘게 나옵니다.

            double maxError = 0;
            double sumError = 0;
            int count = 0;

            for (int i = 0; i < corners.Length - 1; i++)
            {
                // 같은 행에 있는 점끼리만 비교 (줄바꿈 지점 제외)
                if ((i + 1) % PATTERN_W == 0) continue;

                double dPx = Math.Sqrt(Math.Pow(corners[i].X - corners[i + 1].X, 2) + Math.Pow(corners[i].Y - corners[i + 1].Y, 2));

                // 단순 비례식 적용 (픽셀거리 * 배율)
                double dMm = dPx * m_CalX;

                // 실제 값(30mm)과 차이 계산
                double error = Math.Abs(ACTUAL_SQUARE_SIZE - dMm);

                if (error > maxError) maxError = error;
                sumError += error;
                count++;
            }

            // =================================================================================
            // [6. 결과 리포트]
            // =================================================================================
            lstLog.Items.Add($"평균 오차: {(sumError / count):F4} mm");
            lstLog.Items.Add($"최대 오차: {maxError:F4} mm");

            // 시각화: 찾은 코너를 이미지에 그려서 보여줌
            Mat resultImg = _sourceImage.Clone();
            Cv2.DrawChessboardCorners(resultImg, new OpenCvSharp.Size(PATTERN_W, PATTERN_H), corners, found);
            imageBox1.Image = resultImg.ToBitmap();

            MessageBox.Show($"검증 완료!\n\n최대 오차: {maxError:F4} mm\n\n이 오차가 허용 범위를 넘는다면\n반드시 왜곡 보정이 필요합니다.");
        }
    }
}