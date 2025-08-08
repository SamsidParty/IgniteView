import { createRoot } from 'react-dom/client'
import App from './App.jsx'

console.log(window._localStorage._cache);

createRoot(document.getElementById('root')).render(
  <App />,
)
