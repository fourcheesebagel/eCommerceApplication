# eCommmerce Application 

#### For Learning Purposes

This application is built following a tutorial in order to gain insight on how an eCommerce Application can be fully built

After learning most of the back-end and front-end set-ups, additional implementations will be documented in this README file, to expand on the knowledge learned from the tutorial.

This will also include my dumbed-down descriptions for each of the learning points for my own understanding..

## Back-End

<ins>***Key Learning Points***</ins>
- Structure [Restaurant for example]
  - Infrastructure (Data Access Layer)
    - The Storage of our Store! [Contains the connections to our storage, ways we grab stuff from our storage and the dependencies we'll need from the services in our application] 
  - Application (Business Logic Layer) 
    - The Kitchen of our Store! [This layer grabs the 'ingredients' from our Infrastructure, this also defines the services that we'll provide using items from our storage and the ingredients defined in our Domain
  - Host (Main Layer)
    - This Contains our HTTP Endpoints, basically how our users will interact with the back-end [This is the Server of the restaurant, who will received orders to be processed by the Kitchen (Business Logic Layer)] 
  - Domain (Domain Layer) 
    - Defines our application, what am I to you? [What kind of recipes do we have? | Define our Customers? | Define our Servers]
- Validation
  - Microsoft Authentication | Authorization
    - JWT Token Configuration
  - Handling in the Infrastructure Layer, to prevent bad data from entering our database (Can't order something not on our ingredients list)
  - FluentValidation
    - Used to validate incoming User Data for login and creation of user login (Application Layer)
 - Logger
   - Using Serilog to initialize log file writer, to ensure developers have activity logs from the application on a daily basis
 - User Creation and Token Management (Infrastructure)
   - User, Role and Token Management which handles all the calls to our database to either store, create, refresh: Users, Roles or Tokens.
 - Cart and Payment Processing
   - Using Stripe API to process incoming payments and selection of payment methods.
   - Application -> Contains the DTO 'body' for the Checkout, Archiving, GetPaymentMethod, ProcessCarts, all which will be for the front-end user 'interaction' with our system.
     - This will also contain the "Services" we provide to carry out all the different processes for checkout.
   - Domain -> Contains the classes/objects where we define the objects we need for the payment process (Archive[Product we store in our cart], PaymentMethod[Straight-forward])
   -  Host -> Carries out our service to the "Kitchen", will return whether our 'order' was successful or rejected
   -  Infrastructure -> Contains our StripePaymentService which uses the Stripe API, defining our 'standards' or config of payment total display, currency and displaying items.
     - Also in charge of storing any cart items to our database in order for our users to continue storing items in their cart if they choose to come back another time.

## Front-End
