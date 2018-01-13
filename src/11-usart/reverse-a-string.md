# Reverse a string

Alright, next let's make the server more interesting by having it respond to the
client with the reverse of the text that they sent. The server will respond to
the client every time they press the ENTER key. Each server response will be
in a new line.

This time you'll need a buffer; you can use `FixedVec`. Here's the starter code:

``` rust
#![no_std]

extern crate aux;
extern crate heapless;

use heapless::Vec;

fn main() {
    let (_usart1, _mono_timer, _itm) = aux::init();

    let mut buffer: Vec<u8, [u8; 32]> = Vec::new();

    loop {
        buffer.clear();

        // TODO Receive a user request. Each user request ends with ENTER
        // NOTE `buffer.push` returns a `Result`. Handle the error by responding
        // with an error message.

        // TODO Send back the reversed string
    }
}
```
