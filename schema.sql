CREATE TABLE users (
    id integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    email text NOT NULL UNIQUE,
    password_hash text NOT NULL,
    role text NOT NULL DEFAULT 'User',
    is_email_confirmed boolean NOT NULL DEFAULT false,
    created_at timestamp NOT NULL DEFAULT now()
);

CREATE TABLE clients (
    id integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    user_id integer REFERENCES users (id) ON DELETE SET NULL,
    full_name text NOT NULL,
    phone_number text NOT NULL,
    email text,
    admin_tag text,
    admin_note text,
    in_black_list boolean NOT NULL DEFAULT false
);

CREATE TABLE apartments (
    id integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    admin_tag text NOT NULL,
    title text NOT NULL,
    address text NOT NULL,
    description text NOT NULL DEFAULT '',
    guests integer NOT NULL CHECK (guests > 0),
    guest_overpayment numeric(10, 2) NOT NULL DEFAULT 0 CHECK (guest_overpayment >= 0),
    price_per_night numeric(10, 2) NOT NULL CHECK (price_per_night >= 0),
    rooms_count integer NOT NULL CHECK (rooms_count > 0),
    area numeric(8, 2) NOT NULL CHECK (area > 0),
    is_active boolean NOT NULL DEFAULT true,
    is_deleted boolean NOT NULL DEFAULT false,
    main_image text
);

CREATE TABLE gallery_items (
    id integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    apartment_id integer NOT NULL REFERENCES apartments (id) ON DELETE CASCADE,
    image_path text NOT NULL,
    sort_order integer NOT NULL DEFAULT 0
);

CREATE TABLE amenities (
    id integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    name text NOT NULL UNIQUE,
    icon text NOT NULL
);

CREATE TABLE apartment_amenities (
    apartment_id integer NOT NULL REFERENCES apartments (id) ON DELETE CASCADE,
    amenity_id integer NOT NULL REFERENCES amenities (id) ON DELETE CASCADE,
    PRIMARY KEY (apartment_id, amenity_id)
);

CREATE TABLE rules (
    id integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    icon text NOT NULL,
    text text NOT NULL
);

CREATE TABLE apartment_rules (
    apartment_id integer NOT NULL REFERENCES apartments (id) ON DELETE CASCADE,
    rule_id integer NOT NULL REFERENCES rules (id) ON DELETE CASCADE,
    PRIMARY KEY (apartment_id, rule_id)
);

CREATE TABLE wiki_places (
    id integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    name text NOT NULL,
    description text NOT NULL DEFAULT '',
    photo_path text NOT NULL DEFAULT '',
    type text NOT NULL,
    nearby_place_id integer
);

CREATE TABLE nearby_places (
    id integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    icon text NOT NULL,
    name text NOT NULL,
    wiki_place_id integer REFERENCES wiki_places (id) ON DELETE SET NULL
);

ALTER TABLE wiki_places
    ADD CONSTRAINT fk_wiki_places_nearby_place
    FOREIGN KEY (nearby_place_id) REFERENCES nearby_places (id) ON DELETE SET NULL;

CREATE TABLE apartment_nearby_places (
    apartment_id integer NOT NULL REFERENCES apartments (id) ON DELETE CASCADE,
    nearby_place_id integer NOT NULL REFERENCES nearby_places (id) ON DELETE CASCADE,
    distance text,
    PRIMARY KEY (apartment_id, nearby_place_id)
);

CREATE TABLE price_rules (
    id integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    apartment_id integer REFERENCES apartments (id) ON DELETE CASCADE,
    label text NOT NULL,
    start_date date NOT NULL,
    end_date date,
    is_recurring_yearly boolean NOT NULL DEFAULT false,
    multiplier numeric(6, 3),
    fixed_markup numeric(10, 2),
    CHECK (end_date IS NULL OR end_date >= start_date),
    CHECK (multiplier IS NOT NULL OR fixed_markup IS NOT NULL)
);

CREATE TABLE bookings (
    id integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    apartment_id integer NOT NULL REFERENCES apartments (id) ON DELETE RESTRICT,
    client_id integer NOT NULL REFERENCES clients (id) ON DELETE RESTRICT,
    start_date timestamp NOT NULL,
    end_date timestamp NOT NULL,
    total_price numeric(12, 2) NOT NULL DEFAULT 0 CHECK (total_price >= 0),
    status text NOT NULL DEFAULT 'Pending',
    source text NOT NULL DEFAULT 'Site',
    guests integer NOT NULL DEFAULT 1 CHECK (guests > 0),
    created_at timestamp NOT NULL DEFAULT now(),
    nights integer GENERATED ALWAYS AS (end_date::date - start_date::date) STORED,
    CHECK (end_date > start_date)
);

CREATE TABLE external_calendars (
    id integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    apartment_id integer NOT NULL REFERENCES apartments (id) ON DELETE CASCADE,
    source text NOT NULL,
    url text NOT NULL,
    last_synced_at timestamp
);

CREATE TABLE external_bookings (
    id integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    apartment_id integer NOT NULL REFERENCES apartments (id) ON DELETE CASCADE,
    client_id integer REFERENCES clients (id) ON DELETE SET NULL,
    source text NOT NULL,
    start_date timestamp NOT NULL,
    end_date timestamp NOT NULL,
    CHECK (end_date > start_date)
);

CREATE TABLE device_tokens (
    id integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    user_id integer NOT NULL REFERENCES users (id) ON DELETE CASCADE,
    token text NOT NULL UNIQUE,
    created_at timestamp NOT NULL DEFAULT now()
);

CREATE TABLE site_settings (
    id integer PRIMARY KEY DEFAULT 1 CHECK (id = 1),
    header_description text NOT NULL DEFAULT '',
    min_booking_days_ahead integer NOT NULL DEFAULT 0 CHECK (min_booking_days_ahead >= 0),
    max_booking_range_days integer NOT NULL DEFAULT 365 CHECK (max_booking_range_days > 0),
    default_hour_overpayment numeric(10, 2) NOT NULL DEFAULT 0
);

INSERT INTO site_settings (id) VALUES (1);

CREATE INDEX ix_clients_user_id ON clients (user_id);
CREATE INDEX ix_gallery_items_apartment_id ON gallery_items (apartment_id);
CREATE INDEX ix_price_rules_apartment_id ON price_rules (apartment_id);
CREATE INDEX ix_bookings_apartment_dates ON bookings (apartment_id, start_date, end_date);
CREATE INDEX ix_bookings_client_id ON bookings (client_id);
CREATE INDEX ix_bookings_status ON bookings (status);
CREATE INDEX ix_external_bookings_apartment_dates ON external_bookings (apartment_id, start_date, end_date);
CREATE INDEX ix_external_calendars_apartment_id ON external_calendars (apartment_id);
CREATE INDEX ix_nearby_places_wiki_place_id ON nearby_places (wiki_place_id);
