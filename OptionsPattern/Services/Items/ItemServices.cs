using OptionsPattern.Data;
using OptionsPattern.Models.Items;

namespace OptionsPattern.Services.Items
{
    public class ItemServices : IItemServices
    {
        private readonly ApplicationDbContext _db;
        public ItemServices (ApplicationDbContext db)
        {
            _db = db;
        }
       
        public Item AddItem(Item item)
        {
            _db.items.Add(item);
            _db.SaveChanges();
            return item;
        }

        public string DeleteItem(int id)
        {
            try
            {       
                var item = _db.items.SingleOrDefault(x => x.Id == id);
                _db.items.Remove(item!);
                _db.SaveChanges ();
                return "Done";
            }
            catch (Exception ex)
            {
                throw new  Exception(ex.Message);
            }     
        }
        public IEnumerable<Item> GetItems()
        {
            return _db.items.ToList();
        }

        public  Item UpdateItem(int id ,Item item)
        {
            try
            { 
                _db.items.Update(item);
                _db.SaveChanges();
                return item;
            }
            catch (Exception ex)
            {
                return new Item { Message = ex.Message};
            }
        }
    }
}
