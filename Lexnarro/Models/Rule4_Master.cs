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
    
    public partial class Rule4_Master
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Rule4_Master()
        {
            this.State_Category_With_Rule4_Mapping = new HashSet<State_Category_With_Rule4_Mapping>();
        }
    
        public decimal Id { get; set; }
        public string Name { get; set; }
        public int MinUnits { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<State_Category_With_Rule4_Mapping> State_Category_With_Rule4_Mapping { get; set; }
    }
}