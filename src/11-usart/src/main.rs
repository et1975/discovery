#![no_main]
#![no_std]

#[macro_use]
extern crate pg;

use pg::peripheral;

#[inline(never)]
#[no_mangle]
pub fn main() -> ! {
    use pg::time::{FREQUENCY, Instant};
    let usart1 = unsafe { peripheral::usart1_mut() };
    let instant = Instant::now();

    usart1.tdr.write(|w| w.tdr(u16::from('>' as u8)));
    // for x in "The quick brown fox jumps over the lazy dog.\n\r".bytes() {
    //     // Send a single character
    //     usart1.tdr.write(|w| w.tdr(u16::from(x)));
    // }
    for x in b"The quick brown fox jumps over the lazy dog.\n\r".iter() {
        while !usart1.isr.read().txe() {}
        usart1.tdr.write(|w| w.tdr(u16::from(*x)));
    }
    let elapsed = instant.elapsed(); // ticks

    iprintln!("`for` loop took {} ticks ({} us)",
              elapsed,
              elapsed as f32 / FREQUENCY as f32 * 1e6);

    loop {}
}
