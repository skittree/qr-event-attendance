# Automatic Event Registration System Project

Automating the process of registration and attendance of SAS events with a Web-application and an Automatic QR-Code Scanner based on Raspberry Pi.

## Stakeholders

* Event Organizer
* Administration
* Security Staff
* Network Admins
* Event Guests
* University of Tyumen

Currently, the stakeholders expect a prototype system that needs to be greenlit for implementation. The main identified stakeholder is the SAS Event Organizer, whose job as an event organizer at SAS would be greatly simplified by our project. Consequently, the other stakeholders are the SAS administration, security staff and sysadmins, as the project automates their small but not insignificant part in organizing events. It also simplifies the lives of all guests, making them the stakeholder and primary user of our project. We have established contact with Shakhlo and she acts as our POC. We will most likely require funding for the Automatic QR-Code Scanner, the funding stakeholder will most likely be UTMN through SAS. Our POC will contact UTMN to _hopefully_  fund our expenses on the Camera Module and Raspberry Pi.

## Current Process

### Preparation

1. Event organizer makes a registration Google Form
2. Asks the admin to make a form on the website as well
3. Receives registration lists from Google Forms and the website
4. Prints out a combined list
5. Hands the list over to security
6. Organizer monitors registrations and prints out an updated list if necessary

### Arrival

1. Guest arrives
2. Security asks for name and ID to verify their name is on the list
3. Guest can visit the event

Currently, the process of verifying registration for SAS events is held manually by the security and administration staff of the institution. After receiving the list from Google Forms or the SAS website, the administration is responsible for handing an *up-to-date* physical copy of the attendance list to security. If a last second registration occurs, the staff has to print out a new copy and bring it over ASAP. Due to the amount of people involved, the process is _very_ slow and visitors would have to wait or interact with staff just to attend an event they have already registered to. Moreover, registered users have to keep their IDs with themselves to verify their identity to security.

## Our Solution

### Preparation

1. Event organizer creates an event in our Web-application
2. The app creates registration forms automatically
3. App receives registrations and saves them in a local database
4. Automatically sends a QR-Code to a registree's specified email

### Arrival

1. Guest arrives
2. Places the QR-Code at the Automatic QR-Code Scanner
3. Guest can visit the event

Upon registration, users receive a scannable bar/QR-Code they can show at the entrance to be _immediately_ granted entry, avoiding interaction with the staff and simplifying the process. 

In a sense, people would receive a *digital ticket* to an event.
