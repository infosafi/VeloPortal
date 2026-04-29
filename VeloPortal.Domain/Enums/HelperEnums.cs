using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeloPortal.Domain.Enums
{
    public static class HelperEnums
    {
        public enum Action
        {
            Add,
            Update,
            Delete,
            Approval,
            Checked
        }

        public enum Images
        {
            jpg,
            jpeg,
            png,
            PNG

        }

        public enum CodeType
        {
            Accounts,
            Resources

        }
        public enum Channels
        {
            wgatsapp,
            email,
            sms
        }
        public enum TemplateParams
        {
            LeadName,
            NextFollowupDate,
            NextFollowupTime
        }
    }
}
