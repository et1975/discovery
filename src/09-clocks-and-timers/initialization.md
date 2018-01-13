# Initialization

As with every other peripheral, we'll have to initialize this timer before we
can use it. And just as in the previous section, initialization is going to
involve two steps: powering up the timer and then configuring it.

Powering up the timer is easy: We just have to set `TIMEEN` bit to 1. This bit
is in the `APB1ENR` register of the `RCC` register block.

``` rust
// Power on the TIM6 timer
rcc.apb1enr.modify(|_, w| w.tim6en().set_bit());
```

The configuration part is slightly more elaborate.

First, we'll have to configure the timer to operate in one pulse mode.

``` rust
// OPM Select one pulse mode
// CEN Keep the counter disabled for now
tim6.cr1.write(|w| w.opm().clear_bit().cen().set_bit());
```

Then, we'll like to have the `CNT` counter operate at a frequency of 1 KHz
because our `delay` function takes a number of milliseconds as arguments and 1
KHz produces a 1 millisecond period.

``` rust
// Configure the prescaler to have the counter operate at 1 KHz
tim6.psc.write(|w| w.psc().bits(x));
```

I'm going to leave it to you to figure out the value of the prescaler, `x`.
Remember that the frequency of the counter is `APB1_CLOCK / (PSC + 1)` and that
`APB1_CLOCK` is 8 MHz.
