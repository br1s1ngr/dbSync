using AppFramework.AppClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstAppFrameworkApplicationEntities.EntityClasses
{
    class Profile : EntityBase
    {
        protected override string Caption
        {
            get { return "Profile"; }
        }

        protected override Type ListFormType
        {
            get { return typeof(Forms.ProfilesForm); }
        }

        public override string TableName
        {
            get { return "allprofilesss"; }
        }

        protected override string TitleColumn1
        {
            get { return "name"; }
        }

        protected override string TitleColumn2
        {
            get { return "email"; }
        }

        protected override void setupEntityInfo()
        {
            FieldInfoList["name"] = new FieldInfo(true, true, false, "Name", FormDataType.String);
            FieldInfoList["email"] = new FieldInfo(true, true, false, new AppFramework.AppClasses.EDTs.EmailEDT());
        }

    }
}
