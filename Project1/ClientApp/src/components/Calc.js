import React, { useState } from 'react';
import {
    Chart as ChartJS,
    CategoryScale,
    LinearScale,
    BarElement,
    Title,
    Tooltip,
    Legend,
} from 'chart.js';
import { Bar } from 'react-chartjs-2';

export default function Calc(showFunc, ret, labels, histY, show) {

    const close = () => {
        showFunc(false);
    }

    if (!labels.length || !histY.length || !show)
        return (<p>No data<br /><button onClick={close}>Close</button><br /></p>);

    ChartJS.register(
        CategoryScale,
        LinearScale,
        BarElement,
        Title,
        Tooltip,
        Legend
    );

    const options = {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
            legend: {
                position: 'top',
            },
            title: {
                display: true,
                text: 'Histogramm',
            },
        },
    };

    const data = {
        labels,
        datasets: [
            {
                label: 'Days of live',
                data: histY,
                backgroundColor: '#4A9DFF',
            },
        ],
    };

    return (
        <>
            <button onClick={close}>Close</button>
            <div className="info">
            <br />
            <b>Retention:</b> {ret[0]}<br />
            <b>Median:</b> {ret[1]}<br />
            <b>Percentile 10:</b> {ret[2]}<br />
            <b>Percentile 90:</b> {ret[3]}</div>
            <div className="chart"><Bar options={options} data={data}/></div>
        </>
    );
}
