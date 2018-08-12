using System;

namespace Subsystems.ShareQuickComponent.External
{

    public class CMPDirectMessage
    {

        public string type { get; set; }
        public MessageCreate message_create { get; set; }

    }

    public class MessageCreate
    {

        public Target target { get; set; }
        public MessageData message_data { get; set; }
    }

    public class Target
    {

        public string recipient_id { get; set; }
    }

    public class MessageData
    {
        public string text { get; set; }
        public Attachment attachment { get; set; }
    }

    public class Attachment
    {

        public string type { get; set; }
        public Media media { get; set; }

    }

    public class Media
    {

        public string id { get; set; }
    }

}
