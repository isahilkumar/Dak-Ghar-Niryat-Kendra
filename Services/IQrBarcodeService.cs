using DakGharNiryatKendra.Models;

namespace DakGharNiryatKendra.Services
{
    public interface IQrBarcodeService
    {
        string GenerateQrCodeBase64(string text);
        string GenerateQrCodeSvg(string text);
        string GenerateBarcode128Svg(string text, int height = 50);
        string GenerateS10TrackingNumber(ServiceType serviceType);
        string GeneratePbeNumber(string centerCode, PbeType pbeType);
    }
}
