namespace CollectionMapper.RavenDB.AppConsole.Entities
{
    internal class User
    {
        public string Id { get; set; }
        private string OtherID { get; set; }

        public User()
        {
            OtherID = Guid.NewGuid().ToString();
        }
    }
}
