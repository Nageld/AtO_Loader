namespace AtO_Loader
{
    public class ItemDataWrapper : ItemData
    {
        // these must be fields and camel case is for consistency - blame JsonUtility
        public string imageFileName;

        /// <summary>
        /// Fires before MatchManager.CastCard gets called.
        /// </summary>
        public void OnPreCastCard()
        {

        }

        /// <summary>
        /// fires after MatchManager.CastCard gets called.
        /// </summary>
        public void OnPostCastCard()
        {

        }

        /// <summary>
        /// Fires before MatchManager.DiscardCard gets called.
        /// </summary>
        public void OnPreDiscard()
        {

        }

        /// <summary>
        /// Fires after MatchManager.DiscardCard gets called.
        /// </summary>
        public void OnPostDiscard()
        {

        }
    }
}
