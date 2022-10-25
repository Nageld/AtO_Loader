namespace AtO_Loader
{
    public class CardDataWrapper : CardData
    {
        // these must be fields and camel case is for consistency - blame JsonUtility
        public string imageFileName;
        private string upgradesToC;

        public string UpgradesToC
        {
            get => this.upgradesToC;
            set => this.upgradesToC = value;
        }

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
