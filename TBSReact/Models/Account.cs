namespace TbsReact.Models
{
    public class Account
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ServerUrl { get; set; }

        public List<Access> Accesses { get; set; }
    }
}