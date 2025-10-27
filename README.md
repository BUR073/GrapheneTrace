## Data Format

Sensore pressure mapping sensors generate real-time pressure distribution heat maps,
formatted as a time-ordered array of 32x32 matrices.

Data for this live brief project will be
formatted as a series of csv files (figure 1): separated by user ID and time/date.

Values in the database range from 1-255 according to pressure applied to the corresponding
sensor pixel, with 1 being the default zero-force value, scaling linearly with pressure to
saturation at 255.

