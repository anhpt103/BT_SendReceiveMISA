namespace BT_SendDataMISA.Models
{
    public class AccessToken
    {
        public class TokenInfo
        {
            public string access_token { get; set; }
            public string token_type { get; set; }
            public int expires_in { get; set; }
            public string userName { get; set; }
            public string email { get; set; }
            public string fullName { get; set; }
            public string phone { get; set; }
            public string chucVu { get; set; }
            public string maDBHC { get; set; }
            public string tenDBHC { get; set; }
            public string PhanHe { get; set; }
            public string maDiaBanHanhChinhCha { get; set; }
            public string dV_QHNS { get; set; }
            public string loai_dv { get; set; }
            public string cap_dv { get; set; }
        }

        public class TokenParam
        {
            public string username { get; set; }
            public string password { get; set; }
            public string grant_type { get; set; }
            public string client_id { get; set; }
            public string tendiaban { get; set; }

        }
    }
}
