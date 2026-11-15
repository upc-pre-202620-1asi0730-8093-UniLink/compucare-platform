workspace "CompuCare Platform - C4 Model" "Software architecture for the CompuCare Platform: a REST API for UniLink's technical support service for small and medium companies." {

    model {
        // People
        employee = person "Company Employee" "Reports issues with the devices assigned to them and follows their tickets."
        companyAdmin = person "Company Administrator" "Manages locations, equipment, employees, the subscription, and approves quotations."
        technician = person "UniLink Technician" "Diagnoses and resolves assigned tickets and issues quotations."
        support = person "UniLink Support" "Assigns tickets to technicians."

        // External systems
        emailSystem = softwareSystem "Email Service" "Delivers employee invitations and password recovery emails." {
            tags "External"
        }

        // Whole system scope (context level)
        platform = softwareSystem "CompuCare Platform" "Manages company devices, support-hour subscriptions, tickets, and quotations." {

            webApp = container "CompuCare Web Application" "Delivers the Vue.js single-page application to the browser." "HTML, CSS, JavaScript" {
                tags "Directory"
            }

            singlePageApp = container "CompuCare Single Page Application" "Provides the experience of each role in the browser." "Vue 3 and JavaScript" {
                tags "Web Browser"
            }

            // This repository
            api = container "CompuCare.Platform" "Provides backend REST APIs for identity, companies, subscriptions, tickets, quotations, and notifications." "C# and ASP.NET Core" {
                tags "Server-side Application"
                iamComponent = component "IAM Bounded Context" "Company registration, sign-in, employees, technicians, password recovery, and JWT issuance." "ASP.NET Core module"
                companiesComponent = component "Companies Bounded Context" "Companies, locations, equipment, and assignment history." "ASP.NET Core module"
                subscriptionsComponent = component "Subscriptions Bounded Context" "Plans, subscriptions, support-hour consumption, and preventive maintenance." "ASP.NET Core module"
                serviceRequestsComponent = component "Service Requests Bounded Context" "Ticket lifecycle and timeline." "ASP.NET Core module"
                quotationsComponent = component "Quotations Bounded Context" "Quotations, approval, and simulated payment." "ASP.NET Core module"
                notificationsComponent = component "Notifications Bounded Context" "In-app notifications per user." "ASP.NET Core module"
                sharedComponent = component "Shared Bounded Context" "Cross-cutting concerns: error handling, localization, base abstractions, persistence, API docs config." "ASP.NET Core module"
            }

            database = container "CompuCare Database" "Stores all domain data of every bounded context." "MySQL Server" {
                tags "Database"
            }
        }

        // People -> containers
        employee -> webApp "Visits" "HTTPS"
        companyAdmin -> webApp "Visits" "HTTPS"
        technician -> webApp "Visits" "HTTPS"
        support -> webApp "Visits" "HTTPS"

        // Web Application delivers SPA assets to the browser
        webApp -> singlePageApp "Delivers static content and JavaScript" "HTTPS"

        // SPA calls the backend API
        singlePageApp -> api "Signs in, manages devices, tickets, quotations, and the subscription" "JSON/HTTPS"

        // API -> database and external systems
        api -> database "Reads and writes" "Entity Framework Core"
        api -> emailSystem "Sends account emails" "SMTP"

        // Internal component relationships
        iamComponent -> sharedComponent "Uses shared abstractions/utilities"
        companiesComponent -> sharedComponent "Uses shared abstractions/utilities"
        subscriptionsComponent -> sharedComponent "Uses shared abstractions/utilities"
        serviceRequestsComponent -> sharedComponent "Uses shared abstractions/utilities"
        quotationsComponent -> sharedComponent "Uses shared abstractions/utilities"
        notificationsComponent -> sharedComponent "Uses shared abstractions/utilities"

        iamComponent -> companiesComponent "Creates the company of a new administrator through its facade"
        iamComponent -> subscriptionsComponent "Validates the plan and activates the subscription through its facade"
        companiesComponent -> iamComponent "Checks that an employee belongs to the company through its facade"
        companiesComponent -> serviceRequestsComponent "Reads open tickets and ticket history through its facade"
        companiesComponent -> quotationsComponent "Reads quotations for the device history through its facade"
        companiesComponent -> subscriptionsComponent "Reads maintenance visits for the device history through its facade"
        subscriptionsComponent -> companiesComponent "Checks that devices belong to the company through its facade"
        serviceRequestsComponent -> companiesComponent "Checks the device of the requester and reads device names through its facade"
        serviceRequestsComponent -> iamComponent "Checks technicians and reads managers and names through its facade"
        serviceRequestsComponent -> quotationsComponent "Checks unresolved quotations through its facade"
        serviceRequestsComponent -> subscriptionsComponent "Consumes support hours through its facade"
        serviceRequestsComponent -> notificationsComponent "Notifies technicians and managers through its facade"
        quotationsComponent -> serviceRequestsComponent "Reads quotable tickets and pauses them through its facade"
        quotationsComponent -> iamComponent "Reads company managers through its facade"
        quotationsComponent -> notificationsComponent "Notifies managers and technicians through its facade"

        // Persistence access through repository adapters in each component
        iamComponent -> database "Reads and writes IAM data"
        companiesComponent -> database "Reads and writes company and equipment data"
        subscriptionsComponent -> database "Reads and writes subscription data"
        serviceRequestsComponent -> database "Reads and writes ticket data"
        quotationsComponent -> database "Reads and writes quotation data"
        notificationsComponent -> database "Reads and writes notification data"
        iamComponent -> emailSystem "Sends invitations and recovery tokens" "SMTP"

        // Client container to components
        singlePageApp -> iamComponent "Signs in, registers companies, manages employees and accounts" "JSON/HTTPS"
        singlePageApp -> companiesComponent "Manages locations and equipment" "JSON/HTTPS"
        singlePageApp -> subscriptionsComponent "Views plans, balance, and maintenance" "JSON/HTTPS"
        singlePageApp -> serviceRequestsComponent "Reports and works tickets" "JSON/HTTPS"
        singlePageApp -> quotationsComponent "Issues, approves, and pays quotations" "JSON/HTTPS"
        singlePageApp -> notificationsComponent "Reads notifications" "JSON/HTTPS"
    }

    views {
        systemContext platform "SystemContext" {
            include *
            autolayout lr
        }

        container platform "Containers" {
            include *
            autolayout lr
        }

        component api "Components" {
            include *
            autolayout lr
        }

        styles {
            element "Person" {
                shape person
                background #08427b
                color #ffffff
            }
            element "Software System" {
                background #1168bd
                color #ffffff
            }
            element "External" {
                background #999999
                color #ffffff
            }
            element "Container" {
                background #438dd5
                color #ffffff
            }
            element "Component" {
                background #85bbf0
                color #000000
            }
            element "Database" {
                shape cylinder
            }
            element "Web Browser" {
                shape webbrowser
            }
        }
    }
}
