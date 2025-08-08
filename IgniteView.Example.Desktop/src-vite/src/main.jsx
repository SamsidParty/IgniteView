import { createRoot } from 'react-dom/client'
import App from './App.jsx'

console.log(window._localStorageCache);

createRoot(document.getElementById('root')).render(
  <App />,
)
