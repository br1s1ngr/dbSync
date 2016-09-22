using AppFramework.AppClasses;
using AppFramework.AppClasses.EDTs;
using FirstAppFrameworkApplicationEntities.EDTs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstAppFrameworkApplicationEntities.EntityClasses
{
    public class Customers : EntityBase
    {
        protected override string Caption
        {
            get { return "Customers"; }
        }

        protected override Type FormType
        {
            get { return typeof(Forms.CustomersForm); }
        }

        protected override Type ListFormType
        {
            get { throw new NotImplementedException(); }
        }

        public override string TableName
        {
            get { return "customers"; }
        }

        protected override string TitleColumn1
        {
            get { return "CustomerID"; }
        }

        protected override string TitleColumn2
        {
            get { return "Name"; }
        }

        protected override void setupEntityInfo()
        {
            FieldInfoList["CustomerID"] = new FieldInfo(false, false, true, new CustomerEDT());
            FieldInfoList["Name"] = new FieldInfo(false, false, true, new ShortDescriptionEDT());
        }
    }
}
