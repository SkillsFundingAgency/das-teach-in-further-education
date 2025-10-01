using SFA.DAS.TeachInFurtherEducation.Web.Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.TeachInFurtherEducation.Web.Services;

[ExcludeFromCodeCoverage]
public class DropdownMenuService
{
    public List<DropdownMenuItem> GetDropdownMenuItems()
    {
        return new List<DropdownMenuItem>
        {
            new DropdownMenuItem
            {
                Title = "Home",
                Url = "/"
            },
            new DropdownMenuItem
            {
                Title = "Products",
                Url = "/products",
                Children = new List<DropdownMenuItem>
                {
                    new DropdownMenuItem
                    {
                        Title = "Electronics",
                        Url = "/products/electronics",
                        Children = new List<DropdownMenuItem>
                        {
                            new DropdownMenuItem { Title = "Smartphones", Url = "/products/electronics/smartphones" },
                            new DropdownMenuItem { Title = "Laptops", Url = "/products/electronics/laptops" },
                            new DropdownMenuItem { Title = "Tablets", Url = "/products/electronics/tablets" },
                            new DropdownMenuItem { Title = "Accessories", Url = "/products/electronics/accessories" }
                        }
                    },
                    new DropdownMenuItem
                    {
                        Title = "Clothing",
                        Url = "/products/clothing",
                        Children = new List<DropdownMenuItem>
                        {
                            new DropdownMenuItem { Title = "Men", Url = "/products/clothing/men" },
                            new DropdownMenuItem { Title = "Women", Url = "/products/clothing/women" },
                            new DropdownMenuItem { Title = "Kids", Url = "/products/clothing/kids" },
                            new DropdownMenuItem { Title = "Accessories", Url = "/products/clothing/accessories" }
                        }
                    },
                    new DropdownMenuItem
                    {
                        Title = "Home & Garden",
                        Url = "/products/home-garden",
                        Children = new List<DropdownMenuItem>
                        {
                            new DropdownMenuItem { Title = "become a fe teacher", Url = "/become-a-fe-teacher" },
                            new DropdownMenuItem { Title = "Decor", Url = "/products/home-garden/decor" },
                            new DropdownMenuItem { Title = "Kitchen", Url = "/products/home-garden/kitchen" }
                        }
                    }
                }
            },
            new DropdownMenuItem
            {
                Title = "Services",
                Url = "/services",
                Children = new List<DropdownMenuItem>
                {
                    new DropdownMenuItem
                    {
                        Title = "Consulting",
                        Url = "/services/consulting",
                        Children = new List<DropdownMenuItem>
                        {
                            new DropdownMenuItem { Title = "Business Strategy", Url = "/services/consulting/strategy" },
                            new DropdownMenuItem { Title = "IT Consulting", Url = "/services/consulting/it" },
                            new DropdownMenuItem { Title = "Marketing", Url = "/services/consulting/marketing" }
                        }
                    },
                    new DropdownMenuItem
                    {
                        Title = "Support",
                        Url = "/services/support",
                        Children = new List<DropdownMenuItem>
                        {
                            new DropdownMenuItem { Title = "Technical Support", Url = "/services/support/technical" },
                            new DropdownMenuItem { Title = "Customer Service", Url = "/services/support/customer" },
                            new DropdownMenuItem { Title = "Training", Url = "/services/support/training" }
                        }
                    },
                    new DropdownMenuItem { Title = "Maintenance", Url = "/services/maintenance" },
                }
            },
            new DropdownMenuItem
            {
                Title = "Company",
                Url = "/company",
                Children = new List<DropdownMenuItem>
                {
                    new DropdownMenuItem { Title = "About Us", Url = "/company/about" },
                    new DropdownMenuItem { Title = "Team", Url = "/company/team" },
                    new DropdownMenuItem { Title = "Careers", Url = "/company/careers" },
                    new DropdownMenuItem { Title = "Contact", Url = "/company/contact" }
                }
            }
        };
    }
}
