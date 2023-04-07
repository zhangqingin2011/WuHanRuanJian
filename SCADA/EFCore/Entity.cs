using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace SCADA
{
    /// <summary>
    /// 模型基类
    /// </summary>
    public abstract class Entity
    {
        /// <summary>
        /// Id
        /// </summary>
        [Display(Name = nameof(Id))]
        [Required]
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// 创建用户Id
        /// </summary>
        [Display(Name = "创建用户Id")]
        [JsonIgnore]
        public Guid? CreatorUserId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Display(Name = "创建时间")]
        [Required]
        public DateTime CreationTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 最后修改用户Id
        /// </summary>
        [Display(Name = "最后修改用户Id")]
        [JsonIgnore]
        public Guid? LastModifierUserId { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        [Display(Name = "最后修改时间")]
        [JsonIgnore]
        public DateTime? LastModificationTime { get; set; }

        /// <summary>
        /// 软删除
        /// </summary>
        [Display(Name = "软删除")]
        [JsonIgnore]
        [Required]
        public bool IsDeleted { get; set; } = false;

        /// <summary>
        /// 删除用户Id
        /// </summary>
        [Display(Name = "删除用户Id")]
        [JsonIgnore]
        public Guid? DeleterUserId { get; set; }

        /// <summary>
        /// 删除时间
        /// </summary>
        [Display(Name = "删除时间")]
        [JsonIgnore]
        public DateTime? DeletionTime { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Display(Name = "名称")]
        public string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [Display(Name = "描述")]
        public string Description { get; set; }

        /// <summary>
        /// 此对象的全部属性值（JSON）
        /// </summary>
        /// <returns></returns>
        public override string ToString() => JsonConvert.SerializeObject(this, Settings.JsonSerializerSettings);
    }
}

