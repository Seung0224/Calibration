using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using OpenCvSharp; // NuGet: OpenCvSharp4 필수
using OpenCvSharp.Extensions; // NuGet: OpenCvSharp4.Extensions 필수

namespace Calibration
{
    public partial class MainForm : Form
    {
        // OpenCV 샘플 데이터 규격 (실제 현장 데이터에 맞춰 수정 가능)
        private const double ACTUAL_SQUARE_SIZE = 30.0; // mm
        private const int PATTERN_W = 9; // 체커보드 내부 가로 코너 개수
        private const int PATTERN_H = 6; // 체커보드 내부 세로 코너 개수

        private Mat _sourceImage; // 현재 메모리에 로드된 이미지

        public MainForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 이미지 불러오기 버튼 클릭 이벤트
        /// </summary>
        private void BTN_Open_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "이미지 파일|*.jpg;*.jpeg;*.png;*.bmp";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    // 기존 메모리 해제
                    _sourceImage?.Dispose();

                    // 이미지 로드 (OpenCV 방식)
                    _sourceImage = Cv2.ImRead(dlg.FileName);

                    // Cyotek ImageBox에 표시 (Bitmap 변환 필요)
                    imageBox1.Image = _sourceImage.ToBitmap();
                    imageBox1.ZoomToFit();

                    lstLog.Items.Clear();
                    lstLog.Items.Add($"[로드 완료] {dlg.SafeFileName}");
                }
            }
        }

        /// <summary>
        /// 기존 방식(Step 1) 오차 검증 버튼 클릭 이벤트
        /// </summary>
        private void BTN_Verify_Click(object sender, EventArgs e)
        {
            if (_sourceImage == null)
            {
                MessageBox.Show("먼저 이미지를 불러와주세요.");
                return;
            }

            // 1. OpenCV 코너 검출 (PatternSize는 내부 코너 개수 기준)
            bool found = Cv2.FindChessboardCorners(_sourceImage, new OpenCvSharp.Size(PATTERN_W, PATTERN_H), out Point2f[] corners);

            if (!found)
            {
                lstLog.Items.Add("[실패] 코너 검출에 실패했습니다.");
                return;
            }

            // 정밀도 향상을 위한 SubPixel 처리 (최신 머신비전 필수 과정)
            using (Mat gray = _sourceImage.CvtColor(ColorConversionCodes.BGR2GRAY))
            {
                Cv2.CornerSubPix(gray, corners, new OpenCvSharp.Size(11, 11), new OpenCvSharp.Size(-1, -1),
                    new TermCriteria(CriteriaTypes.Eps | CriteriaTypes.MaxIter, 30, 0.1));
            }

            // 2. 기존 C# 방식(Step 1) 재현: 중앙부 두 점을 기준으로 m_CalX 계산
            // 9x6 코너 중 정중앙 부근인 22번과 23번 인덱스 사용
            Point2f pCenter1 = corners[22];
            Point2f pCenter2 = corners[23];

            double pixelDistBase = Math.Sqrt(Math.Pow(pCenter1.X - pCenter2.X, 2) + Math.Pow(pCenter1.Y - pCenter2.Y, 2));
            double m_CalX = ACTUAL_SQUARE_SIZE / pixelDistBase; // mm / pixel

            lstLog.Items.Add($"[기준] m_CalX: {m_CalX:F6} mm/px");
            lstLog.Items.Add("-----------------------------------------");

            // 3. 전체 코너 간 거리 측정 및 오차 분석
            double maxError = 0;
            double sumError = 0;
            int checkCount = 0;

            for (int h = 0; h < PATTERN_H; h++)
            {
                for (int w = 0; w < PATTERN_W - 1; w++)
                {
                    int idx1 = h * PATTERN_W + w;
                    int idx2 = h * PATTERN_W + (w + 1);

                    // 픽셀 거리 계산
                    double distPx = Math.Sqrt(Math.Pow(corners[idx1].X - corners[idx2].X, 2) + Math.Pow(corners[idx1].Y - corners[idx2].Y, 2));

                    // 기존 방식(단순 배율) 적용 거리
                    double distMm = distPx * m_CalX;

                    // 실제 규격(30mm)과의 오차
                    double error = Math.Abs(ACTUAL_SQUARE_SIZE - distMm);

                    sumError += error;
                    if (error > maxError) maxError = error;
                    checkCount++;

                    // 외곽 지역(첫 행과 마지막 행) 로그 출력
                    if (h == 0 || h == PATTERN_H - 1)
                    {
                        lstLog.Items.Add($"R{h} C{w}-{w + 1}: {distMm:F3}mm (Err:{error:F3})");
                    }
                }
            }

            // 4. 결과 출력 및 시각화
            lstLog.Items.Add("-----------------------------------------");
            lstLog.Items.Add($"평균 오차: {sumError / checkCount:F4} mm");
            lstLog.Items.Add($"최대 오차: {maxError:F4} mm");

            // 이미지 위에 검출된 코너 그려서 갱신
            Mat resultView = _sourceImage.Clone();
            Cv2.DrawChessboardCorners(resultView, new OpenCvSharp.Size(PATTERN_W, PATTERN_H), corners, found);
            imageBox1.Image = resultView.ToBitmap();

            MessageBox.Show($"검증이 완료되었습니다.\n최대 오차: {maxError:F4} mm", "검증 결과");
        }
    }
}