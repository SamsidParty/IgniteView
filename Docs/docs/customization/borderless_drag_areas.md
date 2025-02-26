# Custom Drag Areas

Custom drag areas allow you to define where the user is able to drag the window from. They are particularly useful when the [titlebar is disabled](./borderless_0_window.md).

You can make any element into a custom drag area by adding `onmouseover="window.igniteView.dragWindow(event)"` as an attribute:

```html
<button onmouseover="window.igniteView.dragWindow(event)">Drag Me!</button>
```

Now, you are able to drag the button and it will bring the window along with it.

The syntax is similar for different JavaScript frameworks, like React:

```jsx
<button onMouseOver={window.igniteView.dragWindow}>Drag Me!</button>
```