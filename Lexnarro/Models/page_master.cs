//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Lexnarro.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class page_master
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public page_master()
        {
            this.role_page_map = new HashSet<role_page_map>();
        }
    
        public decimal page_id { get; set; }
        public string page_name { get; set; }
        public string page_group { get; set; }
        public string menu_name { get; set; }
        public string status { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<role_page_map> role_page_map { get; set; }
    }
}
