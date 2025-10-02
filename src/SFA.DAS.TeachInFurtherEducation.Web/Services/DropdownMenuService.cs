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
        Title = "FAQs",
        Url = "/faqs",
        Children = new List<DropdownMenuItem>
        {
            new DropdownMenuItem { Title = "Difference Between FE and Higher", Url = "/faqs/difference-between-fe-and-higher" },
            new DropdownMenuItem { Title = "How to Apply", Url = "/faqs/how-to-apply" },
            new DropdownMenuItem { Title = "What is FE", Url = "/faqs/what-is-fe" },
            new DropdownMenuItem { Title = "What is FE Teaching?", Url = "/faqs/what-is-fe-teaching" }
        }
    },
    new DropdownMenuItem
    {
        Title = "Jobs",
        Url = "/jobs",
        Children = new List<DropdownMenuItem>
        {
            new DropdownMenuItem { Title = "AOC", Url = "/jobs/aoc" },
            new DropdownMenuItem
            {
                Title = "Course",
                Url = "/jobs/course",
                Children = new List<DropdownMenuItem>
                {
                    new DropdownMenuItem { Title = "Engineering", Url = "/jobs/course/engineering" },
                    new DropdownMenuItem { Title = "Health", Url = "/jobs/course/health" }
                }
            },
            new DropdownMenuItem { Title = "Location", Url = "/jobs/location" },
            new DropdownMenuItem { Title = "Part-time Tutor", Url = "/jobs/tutor/part-time" }
        }
    },
    new DropdownMenuItem
    {
        Title = "Talk to an Advisor",
        Url = "/talk-to-an-advisor"
    },
    new DropdownMenuItem
    {
        Title = "Teaching",
        Url = "/teaching",
        Children = new List<DropdownMenuItem>
        {
            new DropdownMenuItem { Title = "Building a Career", Url = "/teaching/building-a-career" },
            new DropdownMenuItem { Title = "Daily Life as an FE Teacher", Url = "/teaching/daily-life-as-an-fe-teacher" },
            new DropdownMenuItem
            {
                Title = "Interview Preparation",
                Url = "/teaching/interview-preparation",
                Children = new List<DropdownMenuItem>
                {
                    new DropdownMenuItem { Title = "Step 1", Url = "/teaching/interview-preparation/step-1" },
                    new DropdownMenuItem { Title = "Step 2", Url = "/teaching/interview-preparation/step-2" },
                    new DropdownMenuItem { Title = "Step 3", Url = "/teaching/interview-preparation/step-3" },
                    new DropdownMenuItem { Title = "Step 4", Url = "/teaching/interview-preparation/step-4" },
                    new DropdownMenuItem { Title = "Step 5", Url = "/teaching/interview-preparation/step-5" }
                }
            },
            new DropdownMenuItem { Title = "Part-time Teaching", Url = "/teaching/part-time-teaching" },
            new DropdownMenuItem { Title = "Salary", Url = "/teaching/salary" },
            new DropdownMenuItem { Title = "Teaching Assistant", Url = "/teaching/teaching-assistant" },
            new DropdownMenuItem { Title = "Teaching in Colleges", Url = "/teaching/teaching-in-colleges" },
            new DropdownMenuItem
            {
                Title = "What Courses Can I Teach?",
                Url = "/teaching/what-courses-can-i-teach",
                Children = new List<DropdownMenuItem>
                {
                    new DropdownMenuItem { Title = "Construction", Url = "/teaching/what-courses-can-i-teach/construction" },
                    new DropdownMenuItem { Title = "Digital IT", Url = "/teaching/what-courses-can-i-teach/digital-IT" },
                    new DropdownMenuItem { Title = "English", Url = "/teaching/what-courses-can-i-teach/english" },
                    new DropdownMenuItem { Title = "Maths", Url = "/teaching/what-courses-can-i-teach/maths" }
                }
            }
        }
    },
    new DropdownMenuItem
    {
        Title = "Training",
        Url = "/training",
        Children = new List<DropdownMenuItem>
        {
            new DropdownMenuItem { Title = "Become an FE Teacher", Url = "/training/become-a-fe-teacher" },
            new DropdownMenuItem { Title = "Best Courses", Url = "/training/best-courses" },
            new DropdownMenuItem { Title = "Find Funding", Url = "/training/find-funding" },
            new DropdownMenuItem
            {
                Title = "Qualifications",
                Url = "/training/qualifications",
                Children = new List<DropdownMenuItem>
                {
                    new DropdownMenuItem
                    {
                        Title = "What Courses are Available?",
                        Url = "/training/qualifications/what-courses-are-available",
                        Children = new List<DropdownMenuItem>
                        {
                            new DropdownMenuItem { Title = "PGCE", Url = "/training/qualifications/what-courses-are-available/pgce" }
                        }
                    },
                    new DropdownMenuItem { Title = "How to get Qualified", Url = "/training/qualifications/whats-required-to-become-certified" }
                }
            }
        }
    }
};
    }
}
