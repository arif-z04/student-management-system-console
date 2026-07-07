namespace StudentManagementSystem.Models;


public readonly struct Address
{
    public string Street{ get; }
    public string City { get;}
    public string Village {get;}
    public string PostalCode { get;}
    public string Country { get; }

    public Address(string street, string village, string city, string postalCode, string country){ 
        Street = street;
        Village = village;
        City = city;
        PostalCode = postalCode;
        Country = country;
    }


    public override string ToString()
        => $"{Street}, {Village}, {City} {PostalCode}, {Country}";
}