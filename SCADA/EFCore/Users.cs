using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace SCADA
{
   public  class Users : Entity
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string  username { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string  Password { get; set; }

        public Lever lever { get; set; }

        /// <summary>
        /// 用户角色关联集合
        /// </summary>
        /*[Display(Name = "用户角色关联集合")]
        [JsonIgnore]
        [InverseProperty(nameof(Users))]
        public virtual ICollection<R_User_Role> R_User_Role { get; set; }*/
    }

    public enum Lever
    {
        visit0r,
        Operator,
        Admin,
    }

}
