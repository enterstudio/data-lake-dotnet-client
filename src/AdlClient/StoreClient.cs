namespace AdlClient
{
    public class StoreClient : ClientBase
    {
        public readonly StoreRestClients RestClients;
        public readonly AdlClient.FileSystem.FileSystemCommands FileSystem;

        public StoreClient(Authentication auth, StoreAccount store) :
            base(auth)
        {
            this.RestClients = new StoreRestClients(store, auth);
            this.FileSystem = new AdlClient.FileSystem.FileSystemCommands(store, RestClients);
        }
    }
}