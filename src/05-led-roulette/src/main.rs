#![deny(unsafe_code)]
#![no_main]
#![no_std]

extern crate pg;

use core::iter;
use pg::delay;
use pg::led::LEDS;

#[inline(never)]
#[no_mangle]
pub fn main() -> ! {

    loop {
        for (current, next) in LEDS.iter()
            .zip(LEDS.iter().skip(1).chain(iter::once(&LEDS[0]))) {
            next.on();
            delay::ms(50);
            current.off();
            delay::ms(50);
        }
    }
}
