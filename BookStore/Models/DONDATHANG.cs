//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BookStore.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class DONDATHANG
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DONDATHANG()
        {
            this.CTDATHANGs = new HashSet<CTDATHANG>();
        }
    
        public int SoDH { get; set; }
        public Nullable<int> MaKH { get; set; }
        public Nullable<System.DateTime> NgayDH { get; set; }
        public Nullable<decimal> Trigia { get; set; }
        public Nullable<bool> Dagiao { get; set; }
        public Nullable<System.DateTime> Ngaygiaohang { get; set; }
        public string Tennguoithan { get; set; }
        public string Dienthoainhan { get; set; }
        public Nullable<bool> HTThanhtoan { get; set; }
        public Nullable<bool> HTGiaohang { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CTDATHANG> CTDATHANGs { get; set; }
        public virtual KHACHHANG KHACHHANG { get; set; }
    }
}
