using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleAppRepository
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using (IRepository<Contact> contacts = ContactRepository.Instance)
                {
                    #region Add contact


                    contacts.Add(new Contact(9) { MobileNo = "0123456", FirstName = "Abdul", LastName = "Alim", BirthDate = Convert.ToDateTime("01-Jan-1980"), Group = ContactGroup.Work });

                    #endregion



                    var c2 = contacts.FindById(2);


                    c2.FirstName = "updated name";


                    contacts.Update(c2);


                    Console.WriteLine($"contact {c2.Id} updated successfully");

                    Console.WriteLine(c2.ToString());


                    if (contacts.Delete(c2))
                        Console.WriteLine($"contact {c2.Id} deleted successfully");






                    #region Search from repository

                    var data = contacts.Search("Alim");
                    Console.WriteLine();
                    Console.WriteLine($"Total Contacts {data.Count()}");
                    Console.WriteLine("----------------------------------");

                    foreach (var c in data)
                    {
                        Console.WriteLine(c.ToString());

                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                Console.ReadLine();
            }

        }
    }


    public interface IEntity : IDisposable
    {
        int Id { get; }
        bool IsValid();
    }


    public interface IRepository<T> : IDisposable, IEnumerable<T> where T : IEntity
    {

        IEnumerable<T> Data { get; }
        void Add(T entity);
        bool Delete(T entity);
        void Update(T entity);
        T FindById(int Id);
        IEnumerable<T> Search(string value);

    }


    public enum ContactGroup
    {
        General = 1,
        Family = 8,
        Work = 32

    }


    public sealed class Contact : IEntity
    {
        public int Id { get; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get => $"{this.FirstName} {this.LastName}"; }
        public string MobileNo { get; set; }

        public string Email { get; set; }
        public DateTime? BirthDate { get; set; }
        public ContactGroup Group { get; set; }



        public Contact()
        {

        }
        public Contact(int ContactId)
        {
            this.Id = ContactId;
            this.BirthDate = null;
            this.Group = ContactGroup.General;
        }

        public Contact(int ContactId, string MobileNo, string FirstName, string LastName = null, string Email = null, DateTime? BirthDate = null, ContactGroup Group = ContactGroup.General)
        {
            this.Id = ContactId;
            this.MobileNo = MobileNo;
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.Email = Email;
            this.BirthDate = BirthDate;
            this.Group = Group;
        }


        public bool IsValid()
        {
            bool isValid = true;

            if (string.IsNullOrWhiteSpace(MobileNo))
                isValid = false;
            else if (string.IsNullOrWhiteSpace(FirstName))
                isValid = false;
            else if (string.IsNullOrEmpty(LastName))
                isValid = false;
            else if (this.BirthDate?.Date > DateTime.Now)
                isValid = false;

            return isValid;
        }

        public override string ToString()
        {
            string text = "Contact Info\n";
            text = text + $"Contact ID : {this.Id}\n";
            text += $"Name  : {this.FullName}\n";
            text += $"Mobile  : {this.MobileNo} \n";
            text += $"Email  : {this.Email} \n";
            text += $"Date of Birth : {this.BirthDate?.ToString("d")}\n";
            text += $"Group : {this.Group} \n";
            text += $"************************\n";

            return text;
        }
        public void Dispose()
        {

        }
    }


    public sealed class ContactRepository : IRepository<Contact>
    {


        private static ContactRepository _instance;
        public static ContactRepository Instance
        {
            get
            {
                return  _instance ?? new ContactRepository();;
            }
        }

        List<Contact> Data;

        private ContactRepository()
        {
            Data = new List<Contact>
            {
                new Contact(ContactId: 1, MobileNo: "1234", "Abdur", "Rahman", "abdur@gmail.com"),
                new Contact(ContactId: 2, MobileNo: "", "Abdur", "Rahman", "abdur@gmail.com"),
                new Contact(ContactId: 3, MobileNo: "", "Abdur", "Rahman", "abdur@gmail.com"),
                new Contact(ContactId: 4, MobileNo: "", "Abdur", "Rahman", "abdur@gmail.com"),
                new Contact(ContactId: 5, MobileNo: "", "Abdur", "Rahman", "abdur@gmail.com"),
                new Contact(ContactId: 6, MobileNo: "", "Abdur", "Rahman", "abdur@gmail.com"),
                new Contact(ContactId: 7, MobileNo: "", "Abdur", "Rahman", "abdur@gmail.com")
            };

        }
        public void Dispose()
        {
            this.Data.Clear();
        }


        IEnumerable<Contact> IRepository<Contact>.Data { get; }


        public Contact this[int index]
        {
            get
            {
                return Data[index];
            }
        }

        public void Add(Contact entity)
        {
            if (Data.Any(c => c.Id == entity.Id))
            {
                throw new Exception("Duplicate contact Id, try another");
            }
            else if (entity.IsValid())
            {
                Data.Add(entity);
            }
            else
            {
                throw new Exception("Contact is invalid");
            }
        }

        public bool Delete(Contact entity)
        {
            return Data.Remove(entity);
        }

        public void Update(Contact entity)
        {

            Data[Data.FindIndex(c => c.Id == entity.Id)] = entity;

        }

        public Contact FindById(int Id)
        {
            var result = (from r in Data where r.Id == Id select r).FirstOrDefault();
            return result;
        }

        public IEnumerable<Contact> Search(string value)
        {

            var result = from r in Data
                         where
                         r.Id.ToString().Contains(value) ||
                         r.FirstName.StartsWith(value) ||
                         r.LastName.StartsWith(value) ||
                         r.MobileNo.Contains(value) ||
                         r.Email.Contains(value) ||
                         r.BirthDate.ToString().Contains(value)
                         orderby r.FirstName ascending
                         select r;
            return result;
        }

        public IEnumerator<Contact> GetEnumerator()
        {
            foreach (var c in Data)
            {
                yield return c;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var c in Data)
            {
                yield return c;
            }
        }
    }
}