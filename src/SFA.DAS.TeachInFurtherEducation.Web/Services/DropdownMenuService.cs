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
            CreateMenuItem("Home", "/"),
            CreateMenuItem("FAQs", "/faqs", new List<DropdownMenuItem>
            {
                CreateMenuItem("Difference Between FE and Higher", "/faqs/difference-between-fe-and-higher"),
                CreateMenuItem("How to Apply", "/faqs/how-to-apply"),
                CreateMenuItem("What is FE", "/faqs/what-is-fe"),
                CreateMenuItem("What is FE Teaching?", "/faqs/what-is-fe-teaching")
            }),
            CreateMenuItem("Jobs", "/jobs", new List<DropdownMenuItem>
            {
                CreateMenuItem("AOC", "/jobs/aoc"),
                CreateMenuItem("Course", "/jobs/course", new List<DropdownMenuItem>
                {
                    CreateMenuItem("Engineering", "/jobs/course/engineering"),
                    CreateMenuItem("Health", "/jobs/course/health")
                }),
                CreateMenuItem("Location", "/jobs/location"),
                CreateMenuItem("Part-time Tutor", "/jobs/tutor/part-time")
            }),
            CreateMenuItem("Talk to an Advisor", "/talk-to-an-advisor"),
            CreateMenuItem("Teaching", "/teaching", new List<DropdownMenuItem>
            {
                CreateMenuItem("Building a Career", "/teaching/building-a-career"),
                CreateMenuItem("Daily Life as an FE Teacher", "/teaching/daily-life-as-an-fe-teacher"),
                CreateMenuItem("Interview Preparation", "/teaching/interview-preparation", new List<DropdownMenuItem>
                {
                    CreateMenuItem("Step 1", "/teaching/interview-preparation/step-1"),
                    CreateMenuItem("Step 2", "/teaching/interview-preparation/step-2"),
                    CreateMenuItem("Step 3", "/teaching/interview-preparation/step-3"),
                    CreateMenuItem("Step 4", "/teaching/interview-preparation/step-4"),
                    CreateMenuItem("Step 5", "/teaching/interview-preparation/step-5")
                }),
                CreateMenuItem("Part-time Teaching", "/teaching/part-time-teaching"),
                CreateMenuItem("Salary", "/teaching/salary"),
                CreateMenuItem("Teaching Assistant", "/teaching/teaching-assistant"),
                CreateMenuItem("Teaching in Colleges", "/teaching/teaching-in-colleges"),
                CreateMenuItem("What Courses Can I Teach?", "/teaching/what-courses-can-i-teach", new List<DropdownMenuItem>
                {
                    CreateMenuItem("Construction", "/teaching/what-courses-can-i-teach/construction"),
                    CreateMenuItem("Digital IT", "/teaching/what-courses-can-i-teach/digital-IT"),
                    CreateMenuItem("English", "/teaching/what-courses-can-i-teach/english"),
                    CreateMenuItem("Maths", "/teaching/what-courses-can-i-teach/maths")
                })
            }),
            CreateMenuItem("Training", "/training", new List<DropdownMenuItem>
            {
                CreateMenuItem("Become an FE Teacher", "/training/become-a-fe-teacher"),
                CreateMenuItem("Best Courses", "/training/best-courses"),
                CreateMenuItem("Find Funding", "/training/find-funding"),
                CreateMenuItem("Qualifications", "/training/qualifications", new List<DropdownMenuItem>
                {
                    CreateMenuItem("What Courses are Available?", "/training/qualifications/what-courses-are-available", new List<DropdownMenuItem>
                    {
                        CreateMenuItem("PGCE", "/training/qualifications/what-courses-are-available/pgce")
                    }),
                    CreateMenuItem("How to get Qualified", "/training/qualifications/whats-required-to-become-certified")
                })
            })
        };
    }

    private static DropdownMenuItem CreateMenuItem(string title, string url, List<DropdownMenuItem>? children = null)
    {
        return new DropdownMenuItem
        {
            Title = title,
            Url = url,
            Children = children ?? new List<DropdownMenuItem>()
        };
    }
}