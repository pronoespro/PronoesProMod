using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PronoesProMod
{

    public class DialogPromptInteractableObject
    {
        public string objName;
        public string[] objDialog;
        public string[] dialogSounds;
        public string interactionPropt;

        public DialogPromptInteractableObject(string name,string[] dialog,string[] sounds,string prompt="Interact")
        {
            objName =name;
            objDialog = dialog;
            interactionPropt = prompt;
            dialogSounds = sounds;
        }
    }

}
