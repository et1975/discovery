# `panic!`

The `panic!` macro also sends its output to the ITM!

Change the `main` function to look like this:

``` rust
fn main() {
    panic!("Hello, world!");
}
```

Let's try this program. But before that let's update `.gdbinit` to run that
`monitor tpiu` for us at startup:

```
target remote :3333
load
monitor tpiu config internal itm.txt uart off 8000000
break main
continue
```

```
$ xargo build --target thumbv7em-none-eabihf

$ arm-none-eabi-gdb -q target/thumbv7em-none-eabihf/debug/hello-world
(..)

(gdb) next
11          panic!("Hello, world!")
(gdb) next

Program received signal SIGTRAP, Trace/breakpoint trap.
f3::lang_items::panic_fmt (msg=..., file=..., line=11)
    at $F3/src/lang_items.rs:12
12          bkpt!();

(gdb) _
```

You'll see some new output in `itmdump`'s terminal.

```
# itmdump's terminal
PANIC at 'Hello, world!', src/main.rs:7:5
```

You won't get a `RUST_BACKTRACE` style backtrace in `itmdump`'s output, *but*
you can get the equivalent inside GDB. You already know the command:

```
(gdb) backtrace
#0  rust_begin_unwind (args=..., file=..., line=7, col=5) at aux/src/lib.rs:34
#1  0x08003fbe in core::panicking::panic_fmt ()
#2  0x0800400c in core::panicking::panic ()
#3  0x080001f6 in hello_world::main () at src/main.rs:7
```

Ultimately, `panic!` is just another function call so you can see it leaves
behind a trace of function calls.

Something other interesting thing happened when we hit the `panic!` but you may
have missed it. Let's re-run the program but this time let's use `continue`
instead of `next`:

```
(gdb) monitor reset halt
target state: halted
target halted due to debug-request, current mode: Thread
xPSR: 0x01000000 pc: 0x08000194 msp: 0x2000a000

(gdb) continue
Continuing.

Breakpoint 1, hello_world::main () at $PWD/src/main.rs:10
10      pub fn main() -> ! {
```

We are back in `main`, let's `continue`:

```
(gdb) continue
Continuing.

Program received signal SIGTRAP, Trace/breakpoint trap.
f3::lang_items::panic_fmt (msg=..., file=..., line=11)
    at $F3/src/lang_items.rs:12
12          bkpt!();
```

> Program received signal SIGTRAP, Trace/breakpoint trap.

The program hit a breakpoint! But we didn't set one in GDB. What happened here
is that `panic!` called the `bkpt!()` macro and that `bkpt!` macro *is* a
breakpoint in the form of an instruction. `bkpt!()` actually expands to
`asm!("bkpt")` and `bkpt` is the breakpoint instruction on ARM Cortex-M devices.

Remember that our microcontroller only supports 6 breakpoints? Well, `bkpt!()`
*doesn't* count towards that limit of 6. Only breakpoints set in GDB using the
`break` command count towards that limit. So, feel free to use the `bkpt!`
instruction in your programs from now on. You'll have to wrap it in `unsafe` and
add `#![feature(asm)]` to your crate though because the `asm!` syntax extension
is unstable.

As a final note: Although very useful, ITM is not meant to be used in
*production*. It requires too many components (an extra microcontroller!)
because it can only be used when the microcontroller is attached to a debugger.

Later on, we'll see other simpler communication protocols.
