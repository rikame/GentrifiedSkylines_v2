/*
using GentrifiedSkylines.Redirection;
using ColossalFramework.UI;

namespace GentrifiedSkylines.Detours
{
    [TargetType(typeof(UITextField))]
    class UITextFieldDetour : UITextField
    {
        [RedirectMethod]
        protected internal virtual void OnSubmit()
        {
            bool m_FocusForced = true;
            this.Unfocus();

            if (this.eventTextSubmitted != null)
                this.eventTextSubmitted((UIComponent)this, this.text);
            this.InvokeUpward("OnTextSubmitted", (object)this.text);
        }
    }
}
*/