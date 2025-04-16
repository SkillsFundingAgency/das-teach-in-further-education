## ⛔Never push sensitive information such as client id's, secrets or keys into repositories including in the README file⛔

# das-teach-in-further-education

<img src="https://avatars.githubusercontent.com/u/9841374?s=200&v=4" align="right" alt="UK Government logo">

[![Build Status](https://dev.azure.com/sfa-gov-uk/Digital%20Apprenticeship%20Service/_apis/build/status/_projectname_?branchName=master)](https://dev.azure.com/sfa-gov-uk/Digital%20Apprenticeship%20Service/_build/latest?definitionId=_projectid_&branchName=master)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=_projectId_&metric=alert_status)](https://sonarcloud.io/dashboard?id=_projectId_)
[![Jira Project](https://img.shields.io/badge/Jira-Project-blue)](https://skillsfundingagency.atlassian.net/secure/RapidBoard.jspa?rapidView=564&projectKey=_projectKey_)
[![Confluence Project](https://img.shields.io/badge/Confluence-Project-blue)](https://skillsfundingagency.atlassian.net/wiki/spaces/_pageurl_)
[![License](https://img.shields.io/badge/license-MIT-lightgrey.svg?longCache=true&style=flat-square)](https://en.wikipedia.org/wiki/MIT_License)

## Description

The "Teach in Further Education" service is designed to inspire and support individuals considering a teaching career within the further education (FE) sector. 

The service provides comprehensive information on the various pathways to becoming an FE teacher, detailing required qualifications, training programs, and available financial support such as bursaries and grants. It serves as a centralized resource for understanding the roles and responsibilities of FE teachers, the diversity of subjects taught, and the significant impact they can have on learners of all ages.

## How It Works

The "Teach in Further Education" service builds upon the proven codebase of the "Find an Employment Scheme" platform, delivering a scalable, secure, and efficient solution tailored to its specific objectives.

The service introduces new features, including the ability for users to sign up for email campaigns and perform searches for colleges based on location, supported by a shared SQL database with geospatial querying capabilities. These enhancements provide a more interactive and user-centered experience. Contentful serves as the Content Management System (CMS), enabling admins, content authors, and publishers to create, manage, and publish web content efficiently without developer intervention. 

To support ongoing performance monitoring and user behavior analysis, the platform integrates both Google Analytics via Google Tag Manager and Microsoft Clarity. These tools provide actionable insights through event tracking, heatmaps, and session replays, helping to continuously optimize the user experience. Integration with Mailchimp further enhances engagement by streamlining email campaign management and subscriber signups. Together, these components enable "Teach in Further Education" to deliver a modern, accessible, and user-friendly platform while maintaining high standards of compliance and performance. 

## 🚀 Installation

### Pre-Requisites
```
* A clone of this repository
* A code editor that supports .Net8.0
* An Azure Service Bus instance

```
### Config

This utility uses the standard Apprenticeship Service configuration. All configuration can be found in the [das-employer-config repository](https://github.com/SkillsFundingAgency/das-employer-config).

#### AppSettings.JSON

```
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    }
  },
  "FormOptions": {
    "MaxRequestBodySize": 4194304
  },
  "ConfigurationStorageConnectionString": "UseDevelopmentStorage=true;",
  "ConfigNames": "SFA.DAS.TeachInFurtherEducation.Web",
  "EnvironmentName": "LOCAL",
  "Version": "1.0",
  "APPINSIGHTS_INSTRUMENTATIONKEY": "",
  "AllowedHosts": "*",
  "cdn": {
    "url": "https://das-at-frnt-end.azureedge.net"
  },
  "ConnectionStrings": {
    "Redis": "localhost:6379"
  },
  "NLog": {
    "LogLevel": "Info"
  },
  "Endpoints": {
    "BaseURL": "https://localhost:44318/"
  },
  "ContentUpdates": {
    "Enabled": true,
    "CronSchedule": "0,2 6-23 * * *"
  },
  "SupplierAddressUpdates": {
    "Enabled": true,
    "CronSchedule": "0,2 6-23 * * *"
  },
  "MailChimp": {
    "ApiKey": "",
    "ListId": "",
    "DataCenter": ""
  },
  "ContentfulOptions": {
    "DeliveryApiKey": "",
    "ManagementApiKey": "",
    "PreviewApiKey": "",
    "SpaceId": "",
    "UsePreviewApi": false,
    "MaxNumberOfRateLimitRetries": 0,
    "Environment": "at"
  },
  "GoogleAnalytics": {
    "GoogleTagManagerId": ""
  },
  "MicrosoftClarity": {
    "MsClarityId": ""
  },
  "CSP": {
    "ViolationReportUrl": ""
  },
  "301Redirection": {
    "AppendReferrerOnQueryString":  true,
    "Triggers": [
      {
        "Seq": 500,
        "Exp": "^https?://(?:www\\.)?(?<prefix>(?:at|pp))\\.teach-in-further-education\\.campaign\\.gov\\.uk.*$",
        "Rules": [
          {
            "Seq": 10,
            "Exp": "^https?://(?:www\\.)?(?<prefix>\\w+)\\.teach-in-further-education\\.campaign\\.gov\\.uk(?<port>\\:\\d+)?\\/privacy-policy\\/?.*?$",
            "SendTo": "https://${prefix}-teach-in-further-education.apprenticeships.education.gov.uk${port}/privacy-policies"
          },
          {
            "Seq": 20,
            "Exp": "^https?://(?:www\\.)?(?<prefix>\\w+)\\.teach-in-further-education\\.campaign\\.gov\\.uk(?<port>\\:\\d+)?\\/digital\\/?.*?$",
            "SendTo": "https://${prefix}-teach-in-further-education.apprenticeships.education.gov.uk${port}/Teach-in-Digital-and-IT-sector-page"
          },
          {
            "Seq": 30,
            "Exp": "^https?://(?:www\\.)?(?<prefix>\\w+)\\.teach-in-further-education\\.campaign\\.gov\\.uk(?<port>\\:\\d+)?\\/accessibility-statement\\/?.*?$",
            "SendTo": "https://${prefix}-teach-in-further-education.apprenticeships.education.gov.uk${port}/accessibility-statement"
          },
          {
            "Seq": 40,
            "Exp": "^https?://(?:www\\.)?(?<prefix>\\w+)\\.teach-in-further-education\\.campaign\\.gov\\.uk(?<port>\\:\\d+)?\\/cookies-policy\\/?.*?$",
            "SendTo": "https://${prefix}-teach-in-further-education.apprenticeships.education.gov.uk${port}/CookiePolicy"
          },
          {
            "Seq": 50,
            "Exp": "^https?://(?:www\\.)?(?<prefix>\\w+)\\.teach-in-further-education\\.campaign\\.gov\\.uk(?<port>\\:\\d+)?\\/get-advice\\/?.*?$",
            "SendTo": "https://${prefix}-teach-in-further-education.apprenticeships.education.gov.uk${port}/talk-to-an-adviser"
          },
          {
            "Seq": 60,
            "Exp": "^https?://(?:www\\.)?(?<prefix>\\w+)\\.teach-in-further-education\\.campaign\\.gov\\.uk(?<port>\\:\\d+)?\\/sharing-is-caring\\/?.*?$",
            "SendTo": "https://${prefix}-teach-in-further-education.apprenticeships.education.gov.uk${port}/is-teaching-right-for-me"
          },
          {
            "Seq": 70,
            "Exp": "^https?://(?:www\\.)?(?<prefix>\\w+)\\.teach-in-further-education\\.campaign\\.gov\\.uk(?<port>\\:\\d+)?\\/youre-signed-up\\/?.*?$",
            "SendTo": "https://${prefix}-teach-in-further-education.apprenticeships.education.gov.uk${port}/#signUpForm"
          },
          {
            "Seq": 80,
            "Exp": "^https?://(?:www\\.)?(?<prefix>\\w+)\\.teach-in-further-education\\.campaign\\.gov\\.uk(?<port>\\:\\d+)?\\/what-is-fe-teaching\\/?.*?$",
            "SendTo": "https://${prefix}-teach-in-further-education.apprenticeships.education.gov.uk${port}/is-teaching-right-for-me"
          },
          {
            "Seq": 90,
            "Exp": "^https?://(?:www\\.)?(?<prefix>\\w+)\\.teach-in-further-education\\.campaign\\.gov\\.uk(?<port>\\:\\d+)?\\/matching-trial-for-colleges\\/?.*?$",
            "SendTo": "https://${prefix}-teach-in-further-education.apprenticeships.education.gov.uk${port}"
          },
          {
            "Seq": 100,
            "Exp": "^https?://(?:www\\.)?(?<prefix>\\w+)\\.teach-in-further-education\\.campaign\\.gov\\.uk(?<port>\\:\\d+)?\\/how-do-i-find-a-job-in-fe\\/?.*?$",
            "SendTo": "https://${prefix}-teach-in-further-education.apprenticeships.education.gov.uk${port}/search-for-a-teaching-job"
          },
          {
            "Seq": 110,
            "Exp": "^https?://(?:www\\.)?(?<prefix>\\w+)\\.teach-in-further-education\\.campaign\\.gov\\.uk(?<port>\\:\\d+)?\\/my-skills-were-more-valuable-than-i-realised\\/?.*?$",
            "SendTo": "https://${prefix}-teach-in-further-education.apprenticeships.education.gov.uk${port}/is-teaching-right-for-me"
          },
          {
            "Seq": 120,
            "Exp": "^https?://(?:www\\.)?(?<prefix>\\w+)\\.teach-in-further-education\\.campaign\\.gov\\.uk(?<port>\\:\\d+)?\\/how-a-visit-to-a-further-education-college-changed-peters-career\\/?.*?$",
            "SendTo": "https://${prefix}-teach-in-further-education.apprenticeships.education.gov.uk${port}/teach-construction-sector-page"
          },
          {
            "Seq": 130,
            "Exp": "^https?://(?:www\\.)?(?<prefix>\\w+)\\.teach-in-further-education\\.campaign\\.gov\\.uk(?<port>\\:\\d+)?\\/further-education-was-a-natural-progression-dominics-journey-from-the-raf-to-further-education-teaching\\/?.*?$",
            "SendTo": "https://${prefix}-teach-in-further-education.apprenticeships.education.gov.uk${port}/is-teaching-right-for-me"
          },
          {
            "Seq": 140,
            "Exp": "^https?://(?:www\\.)?(?<prefix>\\w+)\\.teach-in-further-education\\.campaign\\.gov\\.uk(?<port>\\:\\d+)?\\/working-in-further-education-gives-me-more-flexibility-how-christian-found-a-better-work-life-balance\\/?.*?$",
            "SendTo": "https://${prefix}-teach-in-further-education.apprenticeships.education.gov.uk${port}/Teach-in-Digital-and-IT-sector-page"
          },
          {
            "Seq": 150,
            "Exp": "^https?://(?:www\\.)?(?<prefix>\\w+)\\.teach-in-further-education\\.campaign\\.gov\\.uk(?<port>\\:\\d+)?\\/whats-it-like-to-teach-in-fe\\/?.*?$",
            "SendTo": "https://${prefix}-teach-in-further-education.apprenticeships.education.gov.uk${port}/is-teaching-right-for-me"
          },
          {
            "Seq": 160,
            "Exp": "^https?://(?:www\\.)?(?<prefix>\\w+)\\.teach-in-further-education\\.campaign\\.gov\\.uk(?<port>\\:\\d+)?\\/?.*?$",
            "SendTo": "https://${prefix}-teach-in-further-education.apprenticeships.education.gov.uk${port}"
          }
        ]
      },
      {
        "Seq": 100,
        "Exp": "^.*?(?:www\\.)?teach-in-further-education\\.campaign\\.gov\\.uk.*$",
        "Rules": [
          {
            "Seq": 10,
            "Exp": "^https?://(?:www\\.)?teach-in-further-education\\.campaign\\.gov\\.uk(?<port>\\:\\d+)?\\/privacy-policy\\/?.*?$",
            "SendTo": "https://teachinfurthereducation.education.gov.uk${port}/privacy-policies"
          },
          {
            "Seq": 20,
            "Exp": "^https?://(?:www\\.)?teach-in-further-education\\.campaign\\.gov\\.uk(?<port>\\:\\d+)?\\/digital\\/?.*?$",
            "SendTo": "https://teachinfurthereducation.education.gov.uk${port}/Teach-in-Digital-and-IT-sector-page"
          },
          {
            "Seq": 30,
            "Exp": "^https?://(?:www\\.)?teach-in-further-education\\.campaign\\.gov\\.uk(?<port>\\:\\d+)?\\/accessibility-statement\\/?.*?$",
            "SendTo": "https://teachinfurthereducation.education.gov.uk${port}/accessibility-statement"
          },
          {
            "Seq": 40,
            "Exp": "^https?://(?:www\\.)?teach-in-further-education\\.campaign\\.gov\\.uk(?<port>\\:\\d+)?\\/cookies-policy\\/?.*?$",
            "SendTo": "https://teachinfurthereducation.education.gov.uk${port}/CookiePolicy"
          },
          {
            "Seq": 50,
            "Exp": "^https?://(?:www\\.)?teach-in-further-education\\.campaign\\.gov\\.uk(?<port>\\:\\d+)?\\/get-advice\\/?.*?$",
            "SendTo": "https://teachinfurthereducation.education.gov.uk${port}/talk-to-an-adviser"
          },
          {
            "Seq": 60,
            "Exp": "^https?://(?:www\\.)?teach-in-further-education\\.campaign\\.gov\\.uk(?<port>\\:\\d+)?\\/sharing-is-caring\\/?.*?$",
            "SendTo": "https://teachinfurthereducation.education.gov.uk${port}/is-teaching-right-for-me"
          },
          {
            "Seq": 70,
            "Exp": "^https?://(?:www\\.)?teach-in-further-education\\.campaign\\.gov\\.uk(?<port>\\:\\d+)?\\/youre-signed-up\\/?.*?$",
            "SendTo": "https://teachinfurthereducation.education.gov.uk${port}/#signUpForm"
          },
          {
            "Seq": 80,
            "Exp": "^https?://(?:www\\.)?teach-in-further-education\\.campaign\\.gov\\.uk(?<port>\\:\\d+)?\\/what-is-fe-teaching\\/?.*?$",
            "SendTo": "https://teachinfurthereducation.education.gov.uk${port}/is-teaching-right-for-me"
          },
          {
            "Seq": 90,
            "Exp": "^https?://(?:www\\.)?teach-in-further-education\\.campaign\\.gov\\.uk(?<port>\\:\\d+)?\\/matching-trial-for-colleges\\/?.*?$",
            "SendTo": "https://teachinfurthereducation.education.gov.uk${port}"
          },
          {
            "Seq": 100,
            "Exp": "^https?://(?:www\\.)?teach-in-further-education\\.campaign\\.gov\\.uk(?<port>\\:\\d+)?\\/how-do-i-find-a-job-in-fe\\/?.*?$",
            "SendTo": "https://teachinfurthereducation.education.gov.uk${port}/search-for-a-teaching-job"
          },
          {
            "Seq": 110,
            "Exp": "^https?://(?:www\\.)?teach-in-further-education\\.campaign\\.gov\\.uk(?<port>\\:\\d+)?\\/my-skills-were-more-valuable-than-i-realised\\/?.*?$",
            "SendTo": "https://teachinfurthereducation.education.gov.uk${port}/is-teaching-right-for-me"
          },
          {
            "Seq": 120,
            "Exp": "^https?://(?:www\\.)?teach-in-further-education\\.campaign\\.gov\\.uk(?<port>\\:\\d+)?\\/how-a-visit-to-a-further-education-college-changed-peters-career\\/?.*?$",
            "SendTo": "https://teachinfurthereducation.education.gov.uk${port}/teach-construction-sector-page"
          },
          {
            "Seq": 130,
            "Exp": "^https?://(?:www\\.)?teach-in-further-education\\.campaign\\.gov\\.uk(?<port>\\:\\d+)?\\/further-education-was-a-natural-progression-dominics-journey-from-the-raf-to-further-education-teaching\\/?.*?$",
            "SendTo": "https://teachinfurthereducation.education.gov.uk${port}/is-teaching-right-for-me"
          },
          {
            "Seq": 140,
            "Exp": "^https?://(?:www\\.)?teach-in-further-education\\.campaign\\.gov\\.uk(?<port>\\:\\d+)?\\/working-in-further-education-gives-me-more-flexibility-how-christian-found-a-better-work-life-balance\\/?.*?$",
            "SendTo": "https://teachinfurthereducation.education.gov.uk${port}/Teach-in-Digital-and-IT-sector-page"
          },
          {
            "Seq": 150,
            "Exp": "^https?://(?:www\\.)?teach-in-further-education\\.campaign\\.gov\\.uk(?<port>\\:\\d+)?\\/whats-it-like-to-teach-in-fe\\/?.*?$",
            "SendTo": "https://teachinfurthereducation.education.gov.uk${port}/is-teaching-right-for-me"
          },
          {
            "Seq": 160,
            "Exp": "^https?://(?:www\\.)?teach-in-further-education\\.campaign\\.gov\\.uk(?<port>\\:\\d+)?\\/?.*?$",
            "SendTo": "https://teachinfurthereducation.education.gov.uk${port}"
          }
        ]
      },
      {
        "Seq": 1000,
        "Exp": "^.*teach-in-further-education.local*$",
        "Rules": [
          {
            "Seq": 10,
            "Exp": "^https?://(?:www\\.)?old\\.teach-in-further-education\\.local(?<port>\\:\\d+)?/privacyPolicy.*?$",
            "SendTo": "https://new.teachinfurthereducation.local${port}/privacy-policies"
          }
        ]
      },
      {
        "Seq": 2000,
        "Exp": "^.*?(?:www\\.)?teachinfurthereducation\\.education\\.gov\\.uk.*$",
        "Rules": [
          {
            "Seq": 10,
            "Exp": "^https?://(?:www\\.)?teachinfurthereducation\\.education\\.gov\\.uk(?<port>\\:\\d+)?\\/become-a-fe-teacher\\/?.*?$",
            "SendTo": "https://teachinfurthereducation.education.gov.uk${port}/become-an-fe-teacher"
          }
        ]
      }
    ]
      }
}
```

#### Azure Table Storage config

Row Key: SFA.DAS._1.0

Partition Key: LOCAL

Data:

```json
{
  "ConnectionStrings": {
    "Redis": "localhost:6379"
  },
  "NLog": {
    "LogLevel": "Info"
  },
  "Endpoints": {
    "BaseURL": "https://localhost:44318/"
  },
  "ContentUpdates": {
    "Enabled": true,
    "CronSchedule": "0,30 6-23 * * *"
  },
  "ContentfulOptions": {
    "DeliveryApiKey": "<  O-m  >",
    "ManagementApiKey": "",
    "PreviewApiKey": "<  O-m  >",
    "SpaceId": "<SpaceId>",
    "UsePreviewApi": false,
    "MaxNumberOfRateLimitRetries": 0,
    "Environment": "at"
  },
  "GoogleAnalytics": {
    "GoogleTagManagerId": "<GoogleTagManagerId>"
  },
  "MicrosoftClarity": {
    "MsClarityId": "<MsClarityId>"
  },
  "SupplierAddressUpdates": {
    "Enabled": true,
    "CronSchedule": "0,2 6-23 * * *"
  },
  "SqlDB": {
    "ConnectionString": "<ConnectionString>"
  },
  "MailChimp": {
    "ApiKey": "<ApiKey>",
    "ListId": "<ListId>",
    "DataCenter": "<DataCenter>"
  },
  "CSP": {
    "ViolationReportUrl": "/api/csp-violations"
  },
   "ApplicationConfiguration": {
    "RedisConnectionString": " ",
    "DataProtectionKeysDatabase": ""
  },
}
```
Check [das-employer-config repository](https://github.com/SkillsFundingAgency/das-employer-config) for subscription keys and LEPS codes.
## 🔗 External Dependencies


## Technologies

```
* .NET 8.0  
* ASP.NET Core  
* Application Insights  
* Azure Table Storage Configuration  
* NLog (including Redis target)  
* SEO Helper for ASP.NET Core  
* Rate Limiting (AspNetCoreRateLimit)  
* Azurite (Azure Storage Emulator)  
* Contentful CMS Integration  
* Cronos (Cron job scheduling)  
* NUnit
* Moq
```

## 🐛 Known Issues


```

```
