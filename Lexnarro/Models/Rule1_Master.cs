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
    
    public partial class Rule1_Master
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Rule1_Master()
        {
            this.StateActivitySubActivityWithRule1 = new HashSet<StateActivitySubActivityWithRule1>();
        }
    
        public decimal Id { get; set; }
        public string Name { get; set; }
        public Nullable<decimal> Min { get; set; }
        public Nullable<decimal> Max { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StateActivitySubActivityWithRule1> StateActivitySubActivityWithRule1 { get; set; }
    }
}
