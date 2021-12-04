import { useEffect, useState } from "react"
import { Drawer, IconButton, Grid, Button } from "@mui/material"
import MenuIcon from '@mui/icons-material/Menu';
import ChevronLeftIcon from '@mui/icons-material/ChevronLeft';

import AccountTable from "./AccountTable"
import AccountModal from "./Modal/AccountModal"

import { deleteAccount } from "../../api/Accounts/Account";
import { login, logout, loginAll, logoutAll, getStatus } from "../../api/Accounts/Driver";

const SideBar = ({ selected, setSelected }) => {
    const [open, setOpen] = useState(false);
    const [status, setStatus] = useState(false);

    useEffect( ( ) => {
        if ( selected !== -1) {
            const updateStatus = async () => {
                setStatus(await getStatus(selected))            
            }
            updateStatus();
        }
    }, [selected]) 
    const handleDrawerOpen = () => {
        setOpen(true);
    };

    const handleDrawerClose = () => {
        setOpen(false);
    };
 
    const onDelete = async () => {
        await deleteAccount(selected);
        setSelected(-1)
    }

    const onLog = async () => {
        if (status === true) {
            await logout(selected);
            setStatus(await getStatus(selected))
        }
        else {
            await login(selected);
            setStatus(await getStatus(selected))
        }
    }

    const onLoginAll = async () => {
        await loginAll();
        setSelected(-1)
    }

    const onLogoutAll = async () => {
        await logoutAll();
        setSelected(-1)

    }


    return (
        <>
            <IconButton
                color="inherit"
                aria-label="open drawer"
                onClick={handleDrawerOpen}
                edge="start"
                sx={{ mr: 2, ...(open && { display: 'none' }) }}
            >
                <MenuIcon />
            </IconButton>
            <Drawer
                anchor="left"
                open={open}
            >
                <IconButton onClick={handleDrawerClose}>
                    <ChevronLeftIcon />
                </IconButton>
                <AccountTable selected={selected} setSelected={setSelected} />
                <Grid container style={{ "textAlign": "center" }}>
                    <Grid item xs={12}>
                        <AccountModal editMode={false} setAccID={setSelected} />
                    </Grid>
                    <Grid item xs={6}>
                        <AccountModal editMode={true} accID={selected} setAccID={setSelected} />
                    </Grid>
                    <Grid item xs={6}>
                        <Button onClick={onDelete}>Delete</Button>
                    </Grid>
                    <Grid item xs={12}>
                        <Button onClick={onLog}>{status === true ? "Logout" : "Login"}</Button>
                    </Grid>
                    <Grid item xs={6}>
                        <Button onClick={onLoginAll}>Login all</Button>
                    </Grid>
                    <Grid item xs={6}>
                        <Button onClick={onLogoutAll}>Logout all</Button>
                    </Grid>
                </Grid>
            </Drawer>
        </>
    )
}
export default SideBar;