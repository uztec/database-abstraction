using System;
using System.Collections.Generic;

namespace UzunTec.Utils.DatabaseAbstraction.PostgreSQL.Test
{
    public class UserQueryClient
    {
        private readonly DBUser dbUser;

        public UserQueryClient(DBUser dbUser)
        {
            this.dbUser = dbUser;
        }

        public bool Insert(User user)
        {
            return this.dbUser.Insert(user.UserCode, user.UserName, user.UserCodRef, user.PasswordMd5, user.InputDate, user.Status);
        }

        public bool Delete(params int[] userCodes)
        {
            bool output = true;
            foreach (int userCode in userCodes)
            {
                output &= this.Delete(userCode);
            }

            return output;
        }

        public bool Delete(int userCode)
        {
            return this.dbUser.Delete(userCode);
        }

        public bool Update(int oldCode, User user)
        {
            return this.dbUser.Update(oldCode, user.UserCode, user.UserName, user.UserCodRef, user.PasswordMd5, user.InputDate, user.Status) > 0;
        }

        public User FindByID(int ID)
        {
            return this.BuildObjectFromRecord(this.dbUser.FindByID(ID));
        }

        public User FindByCode(int userCode)
        {
            return this.BuildObjectFromRecord(this.dbUser.FindByCode(userCode));
        }

        public List<User> ListAll()
        {
            return this.dbUser.ListAll().BuildList(this.BuildObjectFromRecord);
        }

        private User BuildObjectFromRecord(DataResultRecord dr)
        {
            return (dr == null) ? null : new User
            {
                UserCode = dr.GetValue<int>("cod_user"),
                UserName = dr.GetString("user_name"),
                UserCodRef = dr.GetNullableValue<long>("cod_user_ref"),
                PasswordMd5 = dr.GetString("password_md5"),
                InputDate = dr.GetValue<DateTime>("input_date"),
                Status = dr.GetEnum<StatusUser>("user_status")
            };
        }
    }
}
