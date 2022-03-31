import React, { useState } from 'react';
import { DataGrid } from '@mui/x-data-grid';
import Calc from '../components/Calc'

let loading = true;

export default function Home() {
    const [datas, setDatas] = useState([])
    const [selectedRow, setSelectedRow] = useState(0)
    const [error, setError] = useState(undefined)
    const [result, setResult] = useState(undefined)
    const [showCalc, setShowCalc] = useState(false);
    const [changed, setChanged] = useState(false);
    const [ret, setRet] = useState([0, 0, 0, 0]);
    const [histX, setHistX] = useState([]);
    const [histY, setHistY] = useState([]);

    if (loading) {
        loading = false;
        populateData();
    }

    async function populateData() {
        fetch('userdata').then(function (response) {
            response.json().then(function (text) {
                if (text.error != undefined)
                    setError(text.error);
                else {
                    let tmp = text.datas.map((x) => ({ userID: x.userID, registerDate: new Date(x.registerDate), lastSeenDate: new Date(x.lastSeenDate) }));
                    setDatas(tmp);
                }
            });
        });
    }

    async function populateCalc() {
        fetch('userdata/calc').then(function (response) {
            response.json().then(function (text) {
                if (text.error != undefined)
                    setError(text.error);
                else {
                    setRet([text.retention, text.median, text.percentile10, text.percentile90]);
                    setHistX(text.histX);
                    setHistY(text.histY);
                }
            });
        });
    }

    const addRow = () => {
        let id = 1;
        datas.forEach((x) => id = x.userID >= id ? x.userID + 1 : id);
        let tmp = [...datas, { userID: id, registerDate: new Date(Date.now()), lastSeenDate: new Date(Date.now()) }];
        setDatas(tmp);
        setSelectedRow(id);
        setChanged(true);
        // � data-grid-pro ����� ������� � ������� ������� api
        setTimeout(() => { document.getElementsByClassName('MuiDataGrid-virtualScroller')[0].scroll(0, tmp.length * 48) }, 333)
    }

    const save = () => {
        const requestOptions = {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(datas)
        };
        fetch('userdata', requestOptions)
            .then(response => {
                if (!response.ok)
                    return response.json().then(text => { throw new Error(Object.values(text.errors).join("; ")) });
                return response.json()
            })
            .then(data => {
                if (data.error) {
                    setResult(undefined);
                    setError(data.error)
                } else {
                    setError(undefined);
                    setResult("Saving: Ok");
                    setChanged(false);
                }
            }).catch((e) => {
                setResult(undefined);
                setError("Error: " + e.message);
            });
    }

    const calc = () => {
        setResult(undefined);
        setError(undefined);
        populateCalc();
        if (error == undefined) setShowCalc(true);
    }

    const processRowUpdate = async (newRow) => {
        let fin = datas.find((x) => x.userID == newRow.userID);
        fin.registerDate = newRow.registerDate;
        fin.lastSeenDate = newRow.lastSeenDate;
        setChanged(true);
        return { ...newRow, isNew: false };
    };

    const renderDatasTable = () => {
        return (
            <div style={{ height: 400, width: '100%' }}>
                <DataGrid
                    experimentalFeatures={{ newEditingApi: true }}
                    getRowId={(row) => row.userID}
                    rows={datas}
                    columns={columns}
                    selectionModel={selectedRow}
                    processRowUpdate={processRowUpdate}
                />
                <button onClick={addRow}>Add row</button>
                <button onClick={save} class="button">Save</button>
                <button onClick={calc} class="button" disabled={changed}>Calculate</button>
            </div>
        );
    }

    const getRet = () => ret;
    const getHX = () => histX;
    const getHY = () => histY;

    const close = () => {
        setError(undefined);
    }

    return (
        <div>
            <div class="header">User data</div>
            <div class="messages">
                {error && <p class="error">{error}<br /><button onClick={close}>Close</button><br /></p>}
                {result && result}
            </div>
            <div class="pages">
                {showCalc ? null : renderDatasTable()}
                {showCalc && Calc(setShowCalc, ret, histX, histY, showCalc)}
            </div>
        </div>
    );
}

const columns = [
    { field: 'userID', headerName: 'User', width: 150 },
    { field: 'registerDate', headerName: 'Registered', width: 150, editable: true, type: 'date', },
    { field: 'lastSeenDate', headerName: 'Last seen', width: 150, editable: true, type: "date", },
];