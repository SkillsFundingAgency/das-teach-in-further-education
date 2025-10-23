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
            CreateMenuItem("Homepage", "/"),
            CreateMenuItem("FAQs", "/faqs", new List<DropdownMenuItem>
            {
                CreateMenuItem("The difference between FE and higher education", "/faqs/difference-between-fe-and-higher"),
                CreateMenuItem("How to apply for an FE teaching role", "/faqs/how-to-apply"),
                CreateMenuItem("What is further education?", "/faqs/what-is-fe"),
                CreateMenuItem("FE Teacher Training", "/faqs/what-is-fe-teaching")
            }),
            CreateMenuItem("Jobs in FE", "/jobs", new List<DropdownMenuItem>
            {
                CreateMenuItem("AoC", "/jobs/aoc"),
                CreateMenuItem("Jobs by course", "/jobs/course", new List<DropdownMenuItem>
                {
                    CreateMenuItem("Engineering", "/jobs/course/engineering"),
                    CreateMenuItem("Health and life sciences", "/jobs/course/health")
                }),
                CreateMenuItem("Jobs by location", "/jobs/location"),
                CreateMenuItem("Tutor/Part Time Jobs", "/jobs/tutor/part-time")
            }),
            CreateMenuItem("Talk to an advisor", "/talk-to-an-advisor"),
            CreateMenuItem("Teaching", "/teaching", new List<DropdownMenuItem>
            {
                CreateMenuItem("Building a career", "/teaching/building-a-career"),
                CreateMenuItem("Daily life", "/teaching/daily-life-as-an-fe-teacher"),
                CreateMenuItem("Interview Preparation", "/teaching/interview-preparation", new List<DropdownMenuItem>
                {
                    CreateMenuItem("Step 1: Thinking about what you want from a teaching role", "/teaching/interview-preparation/step-1"),
                    CreateMenuItem("Step 2: Write an updated CV", "/teaching/interview-preparation/step-2"),
                    CreateMenuItem("Step 3: Start your job search", "/teaching/interview-preparation/step-3"),
                    CreateMenuItem("Step 4: How to prepare for an interview", "/teaching/interview-preparation/step-4"),
                    CreateMenuItem("Step 5: After the interview", "/teaching/interview-preparation/step-5")
                }),
                CreateMenuItem("Working part time", "/teaching/part-time-teaching"),
                CreateMenuItem("Salary", "/teaching/salary"),
                CreateMenuItem("FE teaching assistants", "/teaching/teaching-assistant"),
                CreateMenuItem("Teaching in colleges", "/teaching/teaching-in-colleges"),
                CreateMenuItem("What subjects could I teach?", "/teaching/what-courses-can-i-teach", new List<DropdownMenuItem>
                {
                    CreateMenuItem("Construction", "/teaching/what-courses-can-i-teach/construction"),
                    CreateMenuItem("Digital", "/teaching/what-courses-can-i-teach/digital-IT"),
                    CreateMenuItem("Maths", "/teaching/what-courses-can-i-teach/maths"),
                    CreateMenuItem("English", "/teaching/what-courses-can-i-teach/english")
                })
            }),
            CreateMenuItem("Training", "/training", new List<DropdownMenuItem>
            {
                CreateMenuItem("How to become an FE teacher", "/training/become-a-fe-teacher"),
                CreateMenuItem("Best courses for me/if you want to apply to be a teacher", "/training/best-courses"),
                CreateMenuItem("Find funding", "/training/find-funding"),
                CreateMenuItem("Qualifications", "/training/qualifications"),
                CreateMenuItem("What Courses are Available?", "/training/qualifications/what-courses-are-available", new List<DropdownMenuItem>
                {
                    CreateMenuItem("PGCE", "/training/qualifications/what-courses-are-available/pgce")
                }),
                CreateMenuItem("How to get qualified", "/training/qualifications/whats-required-to-become-certified")
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