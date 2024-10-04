using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Quik_BookingApp.Helper
{
    public class VnPayLibrary
    {
        private readonly SortedList<string, string> _requestData = new SortedList<string, string>();
        private readonly SortedList<string, string> _responseData = new SortedList<string, string>();

        public void AddRequestData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _requestData.Add(key, value);
            }
        }

        public void AddResponseData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _responseData.Add(key, value);
            }
        }

        public SortedList<string, string> GetResponseData()
        {
            return _responseData;
        }

        public string CreateRequestUrl(string baseUrl, string hashSecret)
        {
            var data = string.Join("&", _requestData.Select(kv => $"{kv.Key}={HttpUtility.UrlEncode(kv.Value)}"));
            var hashData = $"{data}&{hashSecret}";

            var vnp_SecureHash = GenerateSHA256(hashData);
            var paymentUrl = $"{baseUrl}?{data}&vnp_SecureHash={vnp_SecureHash}";

            return paymentUrl;
        }

        public bool ValidateSignature(SortedList<string, string> responseData, string hashSecret)
        {
            var secureHash = responseData["vnp_SecureHash"];
            responseData.Remove("vnp_SecureHash");

            var data = string.Join("&", responseData.Select(kv => $"{kv.Key}={HttpUtility.UrlEncode(kv.Value)}"));
            var hashData = $"{data}&{hashSecret}";

            var calculatedHash = GenerateSHA256(hashData);
            return secureHash.Equals(calculatedHash, StringComparison.InvariantCultureIgnoreCase);
        }

        private string GenerateSHA256(string input)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(input);
                var hashBytes = sha256.ComputeHash(bytes);

                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
    }

}
