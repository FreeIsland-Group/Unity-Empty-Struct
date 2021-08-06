using System.Collections.Generic;
using DefaultNamespace.Data.Struct;

namespace DefaultNamespace.Data.API
{
    // 登录\注册所需类型
    public class APIAccount
    {
        public User user;
        public Extra extra;

        public class User
        {
            public int id;
            public int point;
            public string name;
            public string api_token;
        }

        public class Extra
        {
            public int ver;
            public string name = "";
            public int point;

            public int status;
            public int signing;
            public List<int> heroCold;
            public int renameCold;
            public List<SaveHero> heroSave;
            public string uSave;
            public string updatedTime;
        }
    }
}
