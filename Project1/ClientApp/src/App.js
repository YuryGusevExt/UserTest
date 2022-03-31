import React, { Component, useState } from 'react';
import Home from './components/Home';

import './custom.css'

export default class App extends Component {
    static displayName = App.name;

    render() {
        return (
            <div class="main">
                <Home />
            </div>
        );
    }
}
