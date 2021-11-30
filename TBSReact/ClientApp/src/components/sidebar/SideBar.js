import React from "react"
import { Drawer, IconButton } from "@mui/material"
import MenuIcon from '@mui/icons-material/Menu';
import ChevronLeftIcon from '@mui/icons-material/ChevronLeft';

import AccountTable from "./AccountTable"
import AccountModal from "./Modal/AccountModal"
const SideBar = ({ selected, setSelected }) => {
    const [open, setOpen] = React.useState(false);
    const handleDrawerOpen = () => {
        setOpen(true);
    };

    const handleDrawerClose = () => {
        setOpen(false);
    };


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
                <AccountModal editMode={true}/>
                <AccountModal editMode={false}/>
            </Drawer>
        </>
    )
}
export default SideBar;