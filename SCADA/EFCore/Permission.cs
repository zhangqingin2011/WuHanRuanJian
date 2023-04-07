using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace SCADA
{
    /// <summary>
    /// 权限
    /// </summary>
    public class Permission : Entity
    {
        /// <summary>
        /// HTTP请求方法
        /// </summary>
        [Display(Name = "HTTP请求方法")]
        [Required]
        public string Method { get; set; }

        /// <summary>
        /// 路由
        /// </summary>
        [Display(Name = "路由")]
        [Required]
        public string Path { get; set; }

        /// <summary>
        /// 准许
        /// </summary>
        [Display(Name = "准许")]
        [Required]
        public bool IsGranted { get; set; } = true;

        /// <summary>
        /// 角色权限关联集合
        /// </summary>
        /*[Display(Name = "角色权限关联集合")]
        [JsonIgnore]
        [InverseProperty(nameof(Permission))]
        public virtual ICollection<R_Role_Permission> R_Role_Permissions { get; set; }*/
    }
}
