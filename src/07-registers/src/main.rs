#![no_std]
#![no_main]

extern crate pg;

#[inline(never)]
#[no_mangle]
pub fn main() -> ! {
    use pg::peripheral;

    // Get mutable access to the GPIOE register block
    // `unsafe` because this functions hands over (aliases) `&mut-` references
    let gpioe = unsafe { peripheral::gpioe_mut() };

    // Turn on the North LED
    gpioe.bsrr.write(|w| w.bs9(true));

    // Turn on the East LED
    gpioe.bsrr.write(|w| w.bs11(true));


    loop {}
}
