using AppFramework.AppClasses;
using AppFramework.AppClasses.EDTs;
using FirstAppFrameworkApplicationEntities.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstAppFrameworkApplicationEntities.EntityClasses
{
    class Games : EntityBase
    {
        protected override string Caption
        {
            get { return "Games"; }
        }

        protected override Type ListFormType
        {
            get { return typeof(GamesForm); }
        }

        public override string TableName
        {
            get { return "games"; }
        }

        protected override string TitleColumn1
        {
            get { return "Name"; }
        }

        protected override string TitleColumn2
        {
            get { return "Played"; }
        }

        protected override void setupEntityInfo()
        {
            FieldInfoList["Name"] = new FieldInfo(true, false, true, new NameEDT());
            FieldInfoList["Played"] = new FieldInfo(true, false, false, "Played", FormDataType.Boolean);
        }
    }
}
