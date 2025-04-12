namespace Auto_parts_store
{
    public static class Entities
    {
        private static AutoPartsStoreEntities context;

        public static AutoPartsStoreEntities GetContext()
        {
            if (context == null)
                context = new AutoPartsStoreEntities();
            return context;
        }
    }
}


