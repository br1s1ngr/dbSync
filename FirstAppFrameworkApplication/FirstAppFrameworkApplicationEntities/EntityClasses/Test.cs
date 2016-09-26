using AppFramework.AppClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstAppFrameworkApplicationEntities.EntityClasses
{
    class TestClass : EntityBase
    {
        protected override string Caption
        {
            get { return "TEst Class FOr Db"; }
        }

        protected override Type ListFormType
        {
            get { return typeof(FirstAppFrameworkApplicationEntities.Forms.TestForm); }
        }

        public override string TableName
        {
            get { return "testclass_db"; }
        }

        protected override string TitleColumn1
        {
            get { return "A"; }
        }

        protected override string TitleColumn2
        {
            get { return "B"; }
        }

        protected override void setupEntityInfo()
        {
            FieldInfoList["A"] = new FieldInfo(true, false, false, "A", FormDataType.String);
            FieldInfoList["B"] = new FieldInfo(true, false, false, "B", FormDataType.String);
        }
    }
}
