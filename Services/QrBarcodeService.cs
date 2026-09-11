using System.Text;
using DakGharNiryatKendra.Models;
using QRCoder;

namespace DakGharNiryatKendra.Services
{
    public class QrBarcodeService : IQrBarcodeService
    {
        private static readonly Random _rand = new();

        public string GenerateQrCodeBase64(string text)
        {
            try
            {
                using var qrGenerator = new QRCodeGenerator();
                using var qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
                var qrCode = new PngByteQRCode(qrCodeData);
                byte[] qrCodeBytes = qrCode.GetGraphic(20);
                return $"data:image/png;base64,{Convert.ToBase64String(qrCodeBytes)}";
            }
            catch
            {
                // Fallback SVG representation encoded as base64
                string svg = GenerateQrCodeSvg(text);
                byte[] bytes = Encoding.UTF8.GetBytes(svg);
                return $"data:image/svg+xml;base64,{Convert.ToBase64String(bytes)}";
            }
        }

        public string GenerateQrCodeSvg(string text)
        {
            try
            {
                using var qrGenerator = new QRCodeGenerator();
                using var qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.M);
                var svgQrCode = new SvgQRCode(qrCodeData);
                return svgQrCode.GetGraphic(6);
            }
            catch
            {
                return "<svg width='100' height='100'><rect width='100' height='100' fill='#f1f5f9'/><text x='10' y='50' fill='#64748b' font-size='12'>QR Code</text></svg>";
            }
        }

        public string GenerateBarcode128Svg(string text, int height = 50)
        {
            // Code 128-B Simplified SVG renderer
            // Produce crisp, scalable SVG bars suitable for thermal label printing and screen display
            text = text.Trim().ToUpperInvariant();
            
            // Build pattern of bars and spaces
            var patternBuilder = new StringBuilder();
            
            // Start B pattern
            patternBuilder.Append("11010010000"); // Start B

            int checksum = 104; // Start B value
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                int codeVal = (int)c - 32;
                if (codeVal < 0 || codeVal > 95) codeVal = 0;

                checksum += codeVal * (i + 1);
                patternBuilder.Append(GetCode128Pattern(codeVal));
            }

            // Checksum character
            int checkVal = checksum % 103;
            patternBuilder.Append(GetCode128Pattern(checkVal));

            // Stop pattern
            patternBuilder.Append("1100011101011"); // Stop

            // Convert binary pattern into SVG rects
            string pattern = patternBuilder.ToString();
            int unitWidth = 2;
            int totalWidth = pattern.Length * unitWidth;

            var svg = new StringBuilder();
            svg.Append($"<svg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 {totalWidth} {height + 18}' class='barcode-svg' style='max-width:100%; height:auto;'>");
            svg.Append($"<rect width='{totalWidth}' height='{height + 18}' fill='#ffffff'/>");

            for (int i = 0; i < pattern.Length; i++)
            {
                if (pattern[i] == '1')
                {
                    svg.Append($"<rect x='{i * unitWidth}' y='0' width='{unitWidth}' height='{height}' fill='#000000'/>");
                }
            }

            // Text label under barcode
            svg.Append($"<text x='{totalWidth / 2}' y='{height + 14}' font-family='monospace, Consolas, sans-serif' font-weight='bold' font-size='13' text-anchor='middle' fill='#111827'>{text}</text>");
            svg.Append("</svg>");

            return svg.ToString();
        }

        public string GenerateS10TrackingNumber(ServiceType serviceType)
        {
            // Standard UPU S10 format: 2 letter prefix + 8 digit serial + 1 check digit + "IN" country code
            string prefix = serviceType switch
            {
                ServiceType.EMS_SpeedPost => "EM",
                ServiceType.InternationalAirParcel => "CP",
                ServiceType.ITPS_TrackedPacket => "LP",
                ServiceType.RegisteredAirMail => "RA",
                _ => "IN"
            };

            int serial = _rand.Next(10000000, 99999999);
            int checkDigit = CalculateS10CheckDigit(serial.ToString());

            return $"{prefix}{serial}{checkDigit}IN";
        }

        public string GeneratePbeNumber(string centerCode, PbeType pbeType)
        {
            string typePrefix = pbeType == PbeType.PBE_I ? "PBE-I" : "PBE-II";
            string centerSlug = centerCode.Replace("DNK-", "").Replace("-", "");
            string year = DateTime.UtcNow.Year.ToString();
            int seq = _rand.Next(1000, 9999);

            return $"{typePrefix}/{year}/{centerSlug}/{seq}";
        }

        private static int CalculateS10CheckDigit(string eightDigits)
        {
            // Standard UPU S10 weighting: 8, 6, 4, 2, 3, 5, 9, 7
            int[] weights = { 8, 6, 4, 2, 3, 5, 9, 7 };
            int sum = 0;
            for (int i = 0; i < 8 && i < eightDigits.Length; i++)
            {
                sum += (eightDigits[i] - '0') * weights[i];
            }

            int remainder = sum % 11;
            if (remainder == 0) return 5;
            if (remainder == 1) return 0;
            return 11 - remainder;
        }

        private static string GetCode128Pattern(int val)
        {
            // Standard Code 128 patterns for 0-106
            string[] patterns = {
                "11011001100", "11001101100", "11001100110", "10010011000", "10010001100", "10001001100",
                "10011001000", "10011000100", "10001100100", "11001001000", "11001000100", "11000100100",
                "10110011100", "10011011100", "10011001110", "10111001100", "10011101100", "10011100110",
                "11001110010", "11001011100", "11001001110", "11011100100", "11001110100", "11101101110",
                "11101001100", "11100101100", "11100100110", "11101100100", "11100110100", "11100110010",
                "11011011000", "11011000110", "11000110110", "10100011000", "10001011000", "10001000110",
                "10110001000", "10001101000", "10001100010", "11010001000", "11000101000", "11000100010",
                "10110111000", "10110001110", "10001101110", "10111011000", "10111000110", "10001110110",
                "11101110110", "11010001110", "11000101110", "11011101000", "11011100010", "11011101110",
                "11101011000", "11101000110", "11100010110", "11101101000", "11101100010", "11100011010",
                "11101111010", "11001000010", "11110001010", "10100110000", "10100001100", "10010110000",
                "10010000110", "10000101100", "10000100110", "10110010000", "10110000100", "10011010000",
                "10011000010", "10000110100", "10000110010", "11000010010", "11001010000", "11110111010",
                "11000010100", "10001111010", "10100111100", "10010111100", "10010011110", "10111100100",
                "10011110100", "10011110010", "11110100100", "11110010100", "11110010010", "11011011110",
                "11011110110", "11110110110", "10101111000", "10100011110", "10001011110", "10111101000",
                "10111100010", "11110101000", "11110100010", "10111011110", "10111101110", "11101011110",
                "11110101110", "11010000100", "11010010000", "11010011100", "1100011101011"
            };

            if (val >= 0 && val < patterns.Length) return patterns[val];
            return "11011001100";
        }
    }
}
